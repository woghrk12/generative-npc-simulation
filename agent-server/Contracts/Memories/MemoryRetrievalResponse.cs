namespace AgentServer.Contracts.Memories;

public sealed record MemoryRetrievalResponse(
    bool Ok,
    string AgentId,
    string Query,
    int Count,
    IReadOnlyList<RetrievedMemoryResponse> Memories
);