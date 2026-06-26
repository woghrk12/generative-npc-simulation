using System.Collections.Concurrent;
using AgentServer.Contracts.Observations;
using AgentServer.Models.Observations;

namespace AgentServer.Services.Observations;

public sealed class InMemoryObservationService : IObservationService
{
    #region Variables

    private const int MAX_OBSERVATION_PER_AGENT = 100;

    private readonly ConcurrentDictionary<string, ConcurrentQueue<StoredObservation>> observationByAgent = new(StringComparer.Ordinal);

    #endregion

    #region  IObservationService Implementations

    public ObservationStoreResult Add(AgentObservationRequest observation)
    {
        ArgumentNullException.ThrowIfNull(observation);

        StoredObservation storedObservation = new StoredObservation(Id: Guid.NewGuid(), Observation: observation, ReceivedAtUtc: DateTimeOffset.UtcNow);

        ConcurrentQueue<StoredObservation> queue = observationByAgent.GetOrAdd(observation.AgentId, static _ => new ConcurrentQueue<StoredObservation>());

        queue.Enqueue(storedObservation);

        while (queue.Count > MAX_OBSERVATION_PER_AGENT && queue.TryDequeue(out _)) { }

        return new ObservationStoreResult(StoredObservation: storedObservation, ObservationCount: queue.Count);
    }

    public IReadOnlyList<StoredObservation> GetByAgentId(string agentId)
    {
        if (observationByAgent.TryGetValue(agentId, out var queue) == false)
        {
            return [];
        }

        return queue.ToArray();
    }

    #endregion
}