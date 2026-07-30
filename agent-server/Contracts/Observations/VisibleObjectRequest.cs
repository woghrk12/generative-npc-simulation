using System.ComponentModel.DataAnnotations;
using AgentServer.Validation;

namespace AgentServer.Contracts.Observations;

public sealed record VisibleObjectRequest
{
    [Required(ErrorMessage = "objectId is required.")]
    [NotWhiteSpace(ErrorMessage = "objectId cannot be blank.")]
    public required string ObjectId { get; init; }

    [Required(ErrorMessage = "displayName is required.")]
    [NotWhiteSpace(ErrorMessage = "displayName cannot be blank.")]
    public required string DisplayName { get; init; }

    [Required(ErrorMessage = "LocationId is required.")]
    [NotWhiteSpace(ErrorMessage = "locationId cannot be blank.")]
    public required string LocationId { get; init; }

    [Required(ErrorMessage = "State is required.")]
    [NotWhiteSpace(ErrorMessage = "state cannot be blank.")]
    public required string State { get; init; }
}