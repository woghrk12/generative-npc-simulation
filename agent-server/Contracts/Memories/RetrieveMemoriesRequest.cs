using System.ComponentModel.DataAnnotations;
using AgentServer.Validation;

namespace AgentServer.Contracts.Memories;

public sealed record RetrieveMemoriesRequest
{
    [Required(ErrorMessage = "query is required.")]
    [NotWhiteSpace(ErrorMessage = "query cannot be blank.")]
    public required string Query { get; init; }


    [Range(1, int.MaxValue, ErrorMessage = "topK must be at least 1.")]
    public int TopK { get; init; } = 5;
}