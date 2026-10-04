using AgentServer.Contracts.Memories;

namespace AgentServer.Contracts.Decisions;

public sealed record DecisionResponse(
    bool Ok,
    string AgentId,
    Guid ObservationId,
    int ObservationCount,
    bool MemoryCreated,
    Guid? MemoryId,
    int MemoryCount,
    string MemoryMessage,
    string DecisionQuery,
    string DecisionPrompt,
    AgentActionResponse Action,
    int RetrievedMemoryCount,
    IReadOnlyList<RetrievedMemoryResponse> RetrievedMemories
);