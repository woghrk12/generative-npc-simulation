namespace AgentServer.Contracts.Memories;

public sealed record MemoryResponse(
    Guid MemoryId,
    string Type,
    string Content,
    string GameTime,
    int Importance,
    string ImportanceReason,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset LastAccessedAtUtc
);