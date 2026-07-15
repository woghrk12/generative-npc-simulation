namespace AgentServer.Contracts.Memories;

public sealed record RetrievedMemoryResponse(
    Guid MemoryId,
    string Type,
    string Content,
    string GameTime,
    int Importance,
    string ImportanceReason,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset LastAccessedAtUtc,
    double RecencyScore,
    double ImportanceScore,
    double RelevanceScore,
    double FinalScore
);