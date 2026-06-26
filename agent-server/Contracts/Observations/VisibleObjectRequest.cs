namespace AgentServer.Contracts.Observations;

public sealed record VisibleObjectRequest
{
    public string ObjectId { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string LocationId { get; init; } = string.Empty;

    public string State { get; init; } = string.Empty;
}