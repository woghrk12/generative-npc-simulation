using AgentServer.Contracts.Observations;
using AgentServer.Models.Agents;
using AgentServer.Models.Memories;

namespace AgentServer.Services.Decisions;

public interface IDecisionPromptBuilder
{
    string Build(AgentProfile profile, AgentObservationRequest observation, IReadOnlyList<MemoryRetrievalItem> retrievedMemories);
}