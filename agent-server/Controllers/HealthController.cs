using AgentServer.Contracts.Health;
using Microsoft.AspNetCore.Mvc;

namespace AgentServer.Controllers;

[ApiController]
[Route("health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<HealthResponse>(StatusCodes.Status200OK)]
    public ActionResult<HealthResponse> GetHealth()
    {
        var response = new HealthResponse(status: "Healthy", service: "AgentServer", timestampUtc: DateTimeOffset.UtcNow);

        return Ok(response);
    }
}