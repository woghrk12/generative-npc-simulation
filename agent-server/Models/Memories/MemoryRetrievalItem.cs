namespace AgentServer.Models.Memories;

public sealed record MemoryRetrievalItem(MemoryRecord Memory, double RecencyScore, double ImportanceScore, double RelevanceScore, double FinalScore);