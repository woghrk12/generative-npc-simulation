namespace AgentServer.Contracts.Memories;

public sealed record MemoryResponse(Guid MemoryId, string Type, string Content, string GameTime, DateTimeOffset CreatedAtUtc, DateTimeOffset LastAccessedAtUtc);