namespace AgentServer.Contracts.Memories;

public sealed record MemoryListResponse(bool Ok, string AgentId, int Count, IReadOnlyList<MemoryResponse> Memories);