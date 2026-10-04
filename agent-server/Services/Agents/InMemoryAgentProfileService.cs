using AgentServer.Models.Agents;

namespace AgentServer.Services.Agents;

public sealed class InMemoryAgentProfileService : IAgentProflieService
{
    private readonly Dictionary<string, AgentProfile> profiles = new(StringComparer.Ordinal)
    {
        ["john"] = new AgentProfile(
            AgentId: "john",
            Name: "John",
            Role: "Town resident",
            Description:
                "John is a resident of a small town. " +
                "He usually spends his day moving between his house, the cafe, and the park.",
            CurrentGoal:
                "Maintain a simple daily routine, " +
                "respond to nearby problems, " +
                "and interact with useful objects in the environment.",
            Traits: ["calm", "practical", "routine-oriented"],
            RoutineHints: [
                "If John is at home and there is no urgent issue, he may visit the cafe.",
                "If John is at the cafe and has finished using useful objects, he may go to the park.",
                "If John is at the park, he may eventually return home.",
                "If John sees a dangerous object state, he should handle it before following his routine."
            ]
        )
    };

    public AgentProfile GetByAgentId(string agentId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(agentId);

        if (profiles.TryGetValue(agentId, out var profile) == false || profile == null)
        {
            return new AgentProfile(
                AgentId: agentId,
                Name: agentId,
                Role: "Unknown resident",
                Description: "This agent is a resident of the simulation.",
                CurrentGoal: "Observe the environment and choose a reasonable next action.",
                Traits: ["neutral"],
                RoutineHints: ["Prefer safe and reasonable actions.", "Respond to urgent visible problems first."]
            );
        }

        return profile;
    }
}