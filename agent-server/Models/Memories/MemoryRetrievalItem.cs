namespace AgentServer.Models.Memories;


public sealed record RawMemoryRetrievalItem(MemoryRecord Memory, double RawRecencyScore, double RawImportanceScore, double RawRelevanceScore);
public sealed record MemoryRetrievalItem(MemoryRecord Memory, double RecencyScore, double ImportanceScore, double RelevanceScore, double FinalScore);