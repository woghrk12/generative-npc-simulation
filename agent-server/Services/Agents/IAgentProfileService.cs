using AgentServer.Models.Agents;

namespace AgentServer.Services.Agents;

public interface IAgentProflieService
{
    AgentProfile GetByAgentId(string agentId);
}