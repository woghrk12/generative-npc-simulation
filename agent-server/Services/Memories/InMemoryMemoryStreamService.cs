using System.Collections.Concurrent;
using System.Text;
using AgentServer.Contracts.Observations;
using AgentServer.Models.Memories;

namespace AgentServer.Services.Memories;

public sealed class InMemoryMemoryStreamService : IMemoryStreamService
{
    #region Variables

    private const int MAX_MEMORIES_PER_AGENT = 1_000;

    private readonly IImportanceScorer importanceScorer;

    private readonly ConcurrentDictionary<string, AgentMemoryState> statesByAgent = new(StringComparer.Ordinal);

    #endregion

    public InMemoryMemoryStreamService(IImportanceScorer importanceScorer)
    {
        this.importanceScorer = importanceScorer;
    }

    public ObservationMemoryResult RecordObservation(AgentObservationRequest observation)
    {
        ArgumentNullException.ThrowIfNull(observation);

        var state = statesByAgent.GetOrAdd(observation.AgentId, static _ => new AgentMemoryState());
        var signature = BuildObservationSignature(observation);

        lock (state.SyncRoot)
        {
            if (string.Equals(state.LastObservationSignature, signature, StringComparison.Ordinal))
            {
                return new ObservationMemoryResult(Created: false, Memory: null, MemoryCount: state.Memories.Count, Reason: "The observation is identical to the previous observation.");
            }

            var now = DateTimeOffset.UtcNow;
            var content = BuildObservationContent(observation);
            var importance = importanceScorer.Score(observation, content);

            var memory = new MemoryRecord(
                Id: Guid.NewGuid(),
                AgentId: observation.AgentId,
                Type: MemoryType.Observation,
                Content: content,
                GameTime: observation.GameTime,
                Importance: importance.Score,
                ImportanceReason: importance.Reason,
                CreatedAtUtc: now,
                LastAccessAtUtc: now
            );

            state.Memories.Add(memory);

            if (state.Memories.Count > MAX_MEMORIES_PER_AGENT)
            {
                var removeCount = state.Memories.Count - MAX_MEMORIES_PER_AGENT;

                state.Memories.RemoveRange(index: 0, count: removeCount);
            }

            state.LastObservationSignature = signature;

            return new ObservationMemoryResult(Created: true, Memory: memory, MemoryCount: state.Memories.Count, Reason: "A meaningful change was observed.");
        }
    }

    public IReadOnlyList<MemoryRecord> GetByAgentId(string agentId)
    {
        if (statesByAgent.TryGetValue(agentId, out var state) == false)
        {
            return [];
        }

        lock (state.SyncRoot)
        {
            return state.Memories.ToArray();
        }
    }

    public void UpdateLastAccessed(string agentId, IReadOnlyCollection<Guid> memoryIds, DateTimeOffset accessedAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(agentId);
        ArgumentNullException.ThrowIfNull(memoryIds);

        if (memoryIds.Count == 0)
        {
            return;
        }

        if (statesByAgent.TryGetValue(agentId, out var state) == false)
        {
            return;
        }

        var idSet = memoryIds.ToHashSet();

        lock (state.SyncRoot)
        {
            for (var index = 0; index < state.Memories.Count; index++)
            {
                var memory = state.Memories[index];

                if (idSet.Contains(memory.Id))
                {
                    continue;
                }

                state.Memories[index] = memory with { LastAccessAtUtc = accessedAtUtc };
            }
        }
    }

    private static string BuildObservationSignature(AgentObservationRequest observation)
    {
        var builder = new StringBuilder();

        AppendSignaturePart(builder, observation.CurrentLocationId);
        AppendSignaturePart(builder, observation.Status);
        AppendSignaturePart(builder, observation.CurrentActionType);
        AppendSignaturePart(builder, observation.CurrentTargetId);
        AppendSignaturePart(builder, observation.LastActionType);
        AppendSignaturePart(builder, observation.LastTargetId);

        var orderedObjects = observation.VisibleObjects.OrderBy(item => item.ObjectId, StringComparer.Ordinal);
        foreach (var visibleObject in orderedObjects)
        {
            AppendSignaturePart(builder, visibleObject.ObjectId);
            AppendSignaturePart(builder, visibleObject.LocationId);
            AppendSignaturePart(builder, visibleObject.State);
        }

        return builder.ToString();
    }

    private static void AppendSignaturePart(StringBuilder builder, string? value)
    {
        builder.Append(value?.Trim() ?? string.Empty);
        builder.Append('\u001F');
    }

    private static string BuildObservationContent(AgentObservationRequest observation)
    {
        var builder = new StringBuilder();

        builder.Append($"{observation.AgentName} is at {observation.CurrentLocationId}. The current status is {observation.Status}.");

        if (string.IsNullOrWhiteSpace(observation.CurrentActionType) == false)
        {
            builder.Append($" {observation.AgentName} is currently performing {observation.CurrentActionType}");

            if (string.IsNullOrWhiteSpace(observation.CurrentTargetId) == false)
            {
                builder.Append($" toward {observation.CurrentTargetId}");
            }

            builder.Append(".");

            if (string.IsNullOrWhiteSpace(observation.CurrentActionReason) == false)
            {
                builder.Append($" The reason is: {observation.CurrentActionReason}.");
            }
        }

        var orderedObjects = observation.VisibleObjects.OrderBy(item => item.ObjectId, StringComparer.Ordinal).ToArray();

        if (orderedObjects.Length == 0)
        {
            builder.Append(" No observable objects are nearby.");
        }
        else
        {
            var visibleObjectDescriptions = orderedObjects.Select(item => $"{item.DisplayName} ({item.ObjectId}) is {item.State}");

            builder.Append(" Visible objects: " + string.Join(", ", visibleObjectDescriptions) + ".");
        }

        if (string.IsNullOrWhiteSpace(observation.LastActionType) == false)
        {
            builder.Append($" The last action was {observation.LastActionType}");

            if (string.IsNullOrWhiteSpace(observation.LastTargetId) == false)
            {
                builder.Append($" toward {observation.LastTargetId}");
            }

            builder.Append(".");
        }

        return builder.ToString();
    }

    private sealed class AgentMemoryState
    {
        public object SyncRoot { get; } = new();

        public List<MemoryRecord> Memories { get; } = new();

        public string? LastObservationSignature { set; get; }
    }
}