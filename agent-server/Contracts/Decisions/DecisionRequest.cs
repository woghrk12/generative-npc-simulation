using System.ComponentModel.DataAnnotations;
using AgentServer.Contracts.Observations;

namespace AgentServer.Contracts.Decisions;

public sealed record DecisionRequest
{
    [Required(ErrorMessage = "observation is required.")]
    public required AgentObservationRequest Observation { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "memoryTopK must be at least 1.")]
    public int MemoryTopK { get; init; } = 5;
}