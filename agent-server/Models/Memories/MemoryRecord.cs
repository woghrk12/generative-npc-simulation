namespace AgentServer.Models.Memories;

public sealed record MemoryRecord(
    Guid Id,
    string AgentId,
    MemoryType Type,
    string Content,
    string GameTime,
    int Importance,
    string ImportanceReason,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset LastAccessAtUtc
);