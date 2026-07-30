using System.ComponentModel.DataAnnotations;
using AgentServer.Validation;

namespace AgentServer.Contracts.Observations;

public sealed record AgentObservationRequest
{
    [Required(ErrorMessage = "agentId is required.")]
    [NotWhiteSpace(ErrorMessage = "agentId cannot be blank.")]
    public required string AgentId { get; init; }

    [Required(ErrorMessage = "agentName is required.")]
    [NotWhiteSpace(ErrorMessage = "agentName cannot be blank.")]
    public required string AgentName { get; init; }

    [Required(ErrorMessage = "gameTime is required.")]
    [NotWhiteSpace(ErrorMessage = "gameTime cannot be blank.")]
    public required string GameTime { get; init; }

    [Required(ErrorMessage = "currentLocationId is required.")]
    [NotWhiteSpace(ErrorMessage = "currentLocationId cannot be blank.")]
    public required string CurrentLocationId { get; init; }

    [Required(ErrorMessage = "status is required.")]
    [NotWhiteSpace(ErrorMessage = "status cannot be blank.")]
    public required string Status { get; init; }

    public string CurrentActionType { get; init; } = string.Empty;

    public string CurrentTargetId { get; init; } = string.Empty;

    public string CurrentActionReason { get; init; } = string.Empty;

    public string LastActionType { get; init; } = string.Empty;

    public string LastTargetId { get; init; } = string.Empty;

    public string LastActionReason { get; init; } = string.Empty;

    [Required(ErrorMessage = "visibleObjects is required.")]
    public required IReadOnlyList<VisibleObjectRequest> VisibleObjects { get; init; }
}