namespace AgentServer.Models.Decisions;

public sealed record AgentActionDecision(
    string ActionType,
    string TargetId,
    string Dialogue,
    int DurationMinutes,
    string Reason
);