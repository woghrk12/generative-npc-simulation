using AgentServer.Contracts.Decisions;
using AgentServer.Contracts.Memories;
using AgentServer.Models.Decisions;
using AgentServer.Models.Memories;
using AgentServer.Services.Decisions;
using AgentServer.Services.Memories;
using AgentServer.Services.Observations;
using Microsoft.AspNetCore.Mvc;

namespace AgentServer.Controllers;

[ApiController]
[Route("agents/{agentId}/decisions")]
public sealed class DecisionsController : ControllerBase
{
    private readonly IObservationService observationService;
    private readonly IMemoryStreamService memoryStreamService;
    private readonly IDecisionService decisionService;

    public DecisionsController(IObservationService observationService, IMemoryStreamService memoryStreamService, IDecisionService decisionService)
    {
        this.observationService = observationService;
        this.memoryStreamService = memoryStreamService;
        this.decisionService = decisionService;
    }

    [HttpPost("next-action")]
    [ProducesResponseType<DecisionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public ActionResult<DecisionResponse> DecideNextAction([FromRoute] string agentId, [FromBody] DecisionRequest request)
    {
        var observation = request.Observation;

        if (string.Equals(agentId, observation.AgentId, StringComparison.Ordinal) == false)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Agent Id mismatch", detail: "route agentId and body agentId must match.");
        }

        var observationResult = observationService.Add(observation);
        var memoryResult = memoryStreamService.RecordObservation(observation);
        var decisionResult = decisionService.Decide(observation, request.MemoryTopK);

        var response = new DecisionResponse(
            Ok: true,
            AgentId: observation.AgentId,
            ObservationId: observationResult.StoredObservation.Id,
            ObservationCount: observationResult.ObservationCount,
            MemoryCreated: memoryResult.Created,
            MemoryId: memoryResult.Memory?.Id,
            MemoryCount: memoryResult.MemoryCount,
            MemoryMessage: memoryResult.Reason,
            DecisionQuery: decisionResult.DecisionQuery,
            Action: ToActionResponse(decisionResult.Action),
            RetrievedMemoryCount: decisionResult.RetrievedMemories.Count,
            RetrievedMemories: decisionResult.RetrievedMemories.Select(ToRetrievedMemoryResponse).ToArray()
        );

        Console.WriteLine($"[Decision] agent={observation.AgentId}, action={response.Action.ActionType}, target={response.Action.TargetId}, retrievedMemories={response.RetrievedMemoryCount}");

        return Ok(response);
    }

    private static AgentActionResponse ToActionResponse(AgentActionDecision action)
    {
        return new AgentActionResponse(
            ActionType: action.ActionType,
            TargetId: action.TargetId,
            Dialogue: action.Dialogue,
            DurationMinutes: action.DurationMinutes,
            Reason: action.Reason
        );
    }

    private static RetrievedMemoryResponse ToRetrievedMemoryResponse(MemoryRetrievalItem item)
    {
        var memory = item.Memory;

        return new RetrievedMemoryResponse(
            MemoryId: memory.Id,
            Type: memory.Type.ToString().ToLowerInvariant(),
            Content: memory.Content,
            GameTime: memory.GameTime,
            Importance: memory.Importance,
            ImportanceReason: memory.ImportanceReason,
            CreatedAtUtc: memory.CreatedAtUtc,
            LastAccessedAtUtc: memory.LastAccessAtUtc,
            RecencyScore: RoundScore(item.RecencyScore),
            ImportanceScore: RoundScore(item.ImportanceScore),
            RelevanceScore: RoundScore(item.RelevanceScore),
            FinalScore: RoundScore(item.FinalScore)
        );
    }

    private static double RoundScore(double score) => Math.Round(score, digits: 4, MidpointRounding.AwayFromZero);
}
