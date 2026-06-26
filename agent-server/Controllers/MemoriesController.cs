using AgentServer.Contracts.Memories;
using AgentServer.Services.Memories;
using Microsoft.AspNetCore.Mvc;

namespace AgentServer.Controllers;

[ApiController]
[Route("agents/{agentId}/memories")]
public sealed class MemoriesController : ControllerBase
{
    private readonly IMemoryStreamService memoryStreamService;

    public MemoriesController(IMemoryStreamService memoryStreamService)
    {
        this.memoryStreamService = memoryStreamService;
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
                CreatedAtUtc: memory.CreatedAtUtc,
                LastAccessedAtUtc: memory.LastAccessAtUtc
            )
        ).ToArray();

        return Ok(new MemoryListResponse(Ok: true, AgentId: agentId, Count: responseItems.Length, Memories: responseItems));
    }
}