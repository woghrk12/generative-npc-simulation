using AgentServer.Contracts.Observations;
using AgentServer.Models.Decisions;

namespace AgentServer.Services.Decisions;

public interface IDecisionService
{
    AgentDecisionResult Decide(AgentObservationRequest observation, int memoryTopK);
}