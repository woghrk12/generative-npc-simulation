using AgentServer.Contracts.Memories;
using AgentServer.Models.Memories;
using AgentServer.Services.Memories;
using Microsoft.AspNetCore.Mvc;

namespace AgentServer.Controllers;

[ApiController]
[Route("agents/{agentId}/memories")]
public sealed class MemoriesController : ControllerBase
{
    private readonly IMemoryStreamService memoryStreamService;
    private readonly IMemoryRetrievalService memoryRetrievalService;

    public MemoriesController(IMemoryStreamService memoryStreamService, IMemoryRetrievalService memoryRetrievalService)
    {
        this.memoryStreamService = memoryStreamService;
        this.memoryRetrievalService = memoryRetrievalService;
    }

    [HttpGet]
    [ProducesResponseType<MemoryListResponse>(StatusCodes.Status200OK)]
    public ActionResult<MemoryListResponse> GetAll([FromRoute] string agentId)
    {
        var memories = memoryStreamService.GetByAgentId(agentId);
        var responseItems = memories.Select(
            memory => new MemoryResponse(
                MemoryId: memory.Id,
                Type: memory.Type.ToString().ToLowerInvariant(),
                Content: memory.Content,
                GameTime: memory.GameTime,
                Importance: memory.Importance,
                ImportanceReason: memory.ImportanceReason,
                CreatedAtUtc: memory.CreatedAtUtc,
                LastAccessedAtUtc: memory.LastAccessAtUtc
            )
        ).ToArray();

        var response = new MemoryListResponse(Ok: true, AgentId: agentId, Count: responseItems.Length, Memories: responseItems);

        return Ok(response);
    }

    [HttpPost("retrieve")]
    [ProducesResponseType<MemoryRetrievalResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<MemoryRetrievalResponse> Retrieve([FromRoute] string agentId, [FromBody] RetrieveMemoriesRequest request)
    {
        if (request is null)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid retrieval request", detail: "Request body is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid retrieval request", detail: "query is required.");
        }

        var topK = request.TopK <= 0 ? 5 : request.TopK;
        var retrievedItems = memoryRetrievalService.Retrieve(agentId, request.Query, topK);
        var responseItems = retrievedItems.Select(ToRetrievedResponse).ToArray();

        var response = new MemoryRetrievalResponse(Ok: true, AgentId: agentId, Query: request.Query, Count: responseItems.Length, Memories: responseItems);

        return Ok(response);
    }

    private static RetrievedMemoryResponse ToRetrievedResponse(MemoryRetrievalItem item)
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