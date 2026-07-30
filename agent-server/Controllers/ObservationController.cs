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
        if (string.Equals(agentId, request.AgentId, StringComparison.Ordinal) == false)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Agent Id mismatch", detail: "route agentId and body agentId must match.");
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
            ? $"[Memory Created] agent={request.AgentId}, memoryId={memoryResult.Memory!.Id}, importance={memoryResult.Memory.Importance}, memoryCount={memoryResult.MemoryCount}"
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
            MemoryMessage: memoryResult.Reason,
            MemoryImportance: memoryResult.Memory?.Importance ?? 0,
            MemoryImportanceReaons: memoryResult.Memory?.ImportanceReason ?? string.Empty
        );

        return Created($"/agents/{request.AgentId}/observations/{response.ObservationId}", response);
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
}