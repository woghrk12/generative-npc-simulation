namespace AgentServer.Contracts.Observations;

public sealed record AgentObservationRequest
{
    public string AgentId { get; init; } = string.Empty;

    public string AgentName { get; init; } = string.Empty;

    public string GameTime { get; init; } = string.Empty;

    public string CurrentLocationId { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string CurrentActionType { get; init; } = string.Empty;

    public string CurrentTargetId { get; init; } = string.Empty;

    public string CurrentActionReason { get; init; } = string.Empty;

    public string LastActionType { get; init; } = string.Empty;

    public string LastTargetId { get; init; } = string.Empty;

    public string LastActionReason { get; init; } = string.Empty;

    public IReadOnlyList<VisibleObjectRequest> VisibleObjects { get; init; } = [];
}