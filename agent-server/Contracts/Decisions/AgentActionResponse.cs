namespace AgentServer.Contracts.Decisions;

public sealed record AgentActionResponse(
    string ActionType,
    string TargetId,
    string Dialogue,
    int DurationMinutes,
    string Reason
);