namespace AgentServer.Models.Agents;

public sealed record AgentProfile(
    string AgentId,
    string Name,
    string Role,
    string Description,
    string CurrentGoal,
    IReadOnlyList<string> Traits,
    IReadOnlyList<string> RoutineHints
);