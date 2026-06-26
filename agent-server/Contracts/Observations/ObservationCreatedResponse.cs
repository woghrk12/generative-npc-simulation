namespace AgentServer.Contracts.Observations;

public sealed record ObservationCreatedResponse(
    bool Ok,
    string Message,
    string AgentId,
    int ObservationCount,
    Guid ObservationId,
    DateTimeOffset ReceivedAtUtc,
    bool MemoryCreated,
    Guid? MemoryId,
    int MemoryCount,
    string MemoryMessage
);