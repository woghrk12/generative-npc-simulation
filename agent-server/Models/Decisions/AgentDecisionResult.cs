using AgentServer.Models.Memories;

namespace AgentServer.Models.Decisions;

public sealed record AgentDecisionResult(
    string DecisionQuery,
    AgentActionDecision Action,
    IReadOnlyList<MemoryRetrievalItem> RetrievedMemories
);