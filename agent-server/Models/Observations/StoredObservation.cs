using AgentServer.Contracts.Observations;

namespace AgentServer.Models.Observations;

public sealed record StoredObservation(Guid Id, AgentObservationRequest Observation, DateTimeOffset ReceivedAtUtc);