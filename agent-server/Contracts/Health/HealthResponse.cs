namespace AgentServer.Contracts.Health;

public sealed record HealthResponse(
    string status,
    string service,
    DateTimeOffset timestampUtc
);