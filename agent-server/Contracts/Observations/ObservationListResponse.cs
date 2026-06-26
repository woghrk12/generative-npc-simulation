namespace AgentServer.Contracts.Observations;

public sealed record ObservationListReponse(bool Ok, string AgentId, int Count, IReadOnlyList<StoredObservationResponse> Observations);