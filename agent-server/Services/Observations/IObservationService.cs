using AgentServer.Contracts.Observations;
using AgentServer.Models.Observations;

namespace AgentServer.Services.Observations;

public interface IObservationService
{
    ObservationStoreResult Add(AgentObservationRequest observation);

    IReadOnlyList<StoredObservation> GetByAgentId(string agentId);
}