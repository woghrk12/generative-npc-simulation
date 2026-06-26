namespace AgentServer.Models.Memories;

public sealed record ObservationMemoryResult(bool Created, MemoryRecord? Memory, int MemoryCount, string Reason);