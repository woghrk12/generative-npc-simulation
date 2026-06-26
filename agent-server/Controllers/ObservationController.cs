using AgentServer.Contracts.Observations;
using AgentServer.Services.Memories;
using AgentServer.Services.Observations;
using Microsoft.AspNetCore.Mvc;

namespace AgentServer.Controllers;

[ApiController]
[Route("agents/{agentId}/observations")]
public sealed class ObservationController : ControllerBase
{
    private readonly IObservationService observationService;
    private readonly IMemoryStreamService memoryStreamService;

    public ObservationController(IObservationService observationService, IMemoryStreamService memoryStreamService)
    {
        this.observationService = observationService;
        this.memoryStreamService = memoryStreamService;
    }

    [HttpPost]
    [ProducesResponseType<ObservationCreatedResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public ActionResult<ObservationCreatedResponse> Create([FromRoute] string agentId, [FromBody] AgentObservationRequest request)
    {
        var validationError = ValidateRequest(request);

        if (validationError is not null)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid observation", detail: validationError);
        }

        if (string.Equals(agentId, request.AgentId, StringComparison.Ordinal) == false)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Agent ID mismatch", detail: $"Route agentId `{agentId}` does not match body agentId `{request.AgentId}`.");
        }

        var observationResult = observationService.Add(request);
        var memoryResult = memoryStreamService.RecordObservation(request);

        Console.WriteLine(
            $"[Observation Received] " +
            $"agent={request.AgentId}, " +
            $"gameTime={request.GameTime}, " +
            $"location={request.CurrentLocationId}, " +
            $"visibleObjects={request.VisibleObjects.Count}, " +
            $"count={observationResult.ObservationCount}"
        );

        Console.WriteLine(
            memoryResult.Created
            ? $"[Memory Created] agent={request.AgentId}, memoryId={memoryResult.Memory!.Id}, memoryCount={memoryResult.MemoryCount}"
            : $"[Memory Skipped] agent={request.AgentId}, reason={memoryResult.Reason}, memoryCount={memoryResult.MemoryCount}"
        );

        var response = new ObservationCreatedResponse(
            Ok: true,
            Message: "Observation received",
            AgentId: request.AgentId,
            ObservationCount: observationResult.ObservationCount,
            ObservationId: observationResult.StoredObservation.Id,
            ReceivedAtUtc: observationResult.StoredObservation.ReceivedAtUtc,
            MemoryCreated: memoryResult.Created,
            MemoryId: memoryResult.Memory?.Id,
            MemoryCount: memoryResult.MemoryCount,
            MemoryMessage: memoryResult.Reason
        );

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet]
    [ProducesResponseType<ObservationListReponse>(StatusCodes.Status200OK)]
    public ActionResult<ObservationListReponse> GetAll([FromRoute] string agentId)
    {
        var observations = observationService.GetByAgentId(agentId);
        var responseItems = observations.Select(item => new StoredObservationResponse(ObservationId: item.Id, ReceivedAtUtc: item.ReceivedAtUtc, Observation: item.Observation)).ToArray();
        var response = new ObservationListReponse(Ok: true, AgentId: agentId, Count: responseItems.Length, Observations: responseItems);

        return Ok(response);
    }

    private static string? ValidateRequest(AgentObservationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AgentId))
        {
            return "agentId is required.";
        }

        if (string.IsNullOrWhiteSpace(request.AgentName))
        {
            return "agentName is required.";
        }

        if (string.IsNullOrWhiteSpace(request.GameTime))
        {
            return "gameTime is required.";
        }

        if (string.IsNullOrWhiteSpace(request.CurrentLocationId))
        {
            return "currentLocationId is required.";
        }

        if (string.IsNullOrWhiteSpace(request.Status))
        {
            return "status is required.";
        }

        if (request.VisibleObjects is null)
        {
            return "visibleObjects is required.";
        }

        for (int index = 0; index < request.VisibleObjects.Count; index++)
        {
            var visibleObject = request.VisibleObjects[index];

            if (string.IsNullOrWhiteSpace(visibleObject.ObjectId))
            {
                return $"visibleObjects[{index}].objectId is required.";
            }

            if (string.IsNullOrWhiteSpace(visibleObject.DisplayName))
            {
                return $"visibleObjects[{index}].displayName is required.";
            }

            if (string.IsNullOrWhiteSpace(visibleObject.LocationId))
            {
                return $"visibleObjects[{index}].locationId is required.";
            }

            if (string.IsNullOrWhiteSpace(visibleObject.State))
            {
                return $"visibleObjects[{index}].state is required.";
            }
        }

        return null;
    }
}