namespace AgentServer.Contracts.Memories;

public sealed record RetrieveMemoriesRequest
{
    public string Query { get; init; } = string.Empty;

    public int TopK { get; init; } = 5;
}