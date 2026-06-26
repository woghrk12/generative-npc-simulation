using System.Collections.Concurrent;
using AgentServer.Contracts.Observations;
using AgentServer.Models.Memories;

namespace AgentServer.Services.Memories;

public interface IMemoryStreamService
{
    ObservationMemoryResult RecordObservation(AgentObservationRequest observation);

    IReadOnlyList<MemoryRecord> GetByAgentId(string agentId);
}