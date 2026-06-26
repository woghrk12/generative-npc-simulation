namespace AgentServer.Contracts.Observations;

public sealed record StoredObservationResponse(Guid ObservationId, DateTimeOffset ReceivedAtUtc, AgentObservationRequest Observation);