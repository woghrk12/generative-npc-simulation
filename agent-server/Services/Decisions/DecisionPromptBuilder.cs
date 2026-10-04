using System.Text;
using AgentServer.Contracts.Observations;
using AgentServer.Models.Agents;
using AgentServer.Models.Memories;

namespace AgentServer.Services.Decisions;

public sealed class DecisionPromptBuilder : IDecisionPromptBuilder
{
    public string Build(AgentProfile profile, AgentObservationRequest observation, IReadOnlyList<MemoryRetrievalItem> retrievedMemories)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(observation);
        ArgumentNullException.ThrowIfNull(retrievedMemories);

        var builder = new StringBuilder();

        AppendSystemInstruction(builder);
        AppendAgentProfile(builder, profile);
        AppendCurrentObservation(builder, observation);
        AppendRetrievedMemories(builder, retrievedMemories);
        AppendActionInstructions(builder);

        return builder.ToString();
    }

    private static void AppendSystemInstruction(StringBuilder builder)
    {
        builder.AppendLine("You are deciding the next action for a simulation NPC.");
        builder.AppendLine("Choose one reasonable next action based on the agent profile, current observation, and relevant memories.");
        builder.AppendLine("Do not invent locations or objects that are not present in the provided context.");

        builder.AppendLine();
    }

    private static void AppendAgentProfile(StringBuilder builder, AgentProfile profile)
    {
        builder.AppendLine("# Agent Profile");

        builder.AppendLine($"AgentId: {profile.AgentId}");
        builder.AppendLine($"Name: {profile.Name}");
        builder.AppendLine($"Role: {profile.Role}");
        builder.AppendLine($"Description: {profile.Description}");
        builder.AppendLine($"Current Goal: {profile.CurrentGoal}");

        builder.AppendLine("Traits:");
        foreach (var trait in profile.Traits)
        {
            builder.AppendLine($"- {trait}");
        }

        builder.AppendLine("Routine Hints:");
        foreach (var hint in profile.RoutineHints)
        {
            builder.AppendLine($"- {hint}");
        }

        builder.AppendLine();
    }

    private static void AppendCurrentObservation(StringBuilder builder, AgentObservationRequest observation)
    {
        builder.AppendLine("# Current Observation");

        builder.AppendLine($"Game Time: {observation.GameTime}");
        builder.AppendLine($"Location: {observation.CurrentLocationId}");
        builder.AppendLine($"Status: {observation.Status}");

        if (string.IsNullOrWhiteSpace(observation.CurrentActionType) == false)
        {
            builder.AppendLine($"Current Action: {observation.CurrentActionType}");
            builder.AppendLine($"Current Target: {observation.CurrentTargetId}");
            builder.AppendLine($"Current Action Reason: {observation.CurrentActionReason}");
        }
        else
        {
            builder.AppendLine("Current Action: None");
        }

        if (string.IsNullOrWhiteSpace(observation.LastActionType) == false)
        {
            builder.AppendLine($"Last Action: {observation.LastActionType}");
            builder.AppendLine($"Last Target: {observation.LastTargetId}");
            builder.AppendLine($"Last Action Reason: {observation.LastActionReason}");
        }
        else
        {
            builder.AppendLine("Last Action: None");
        }

        builder.AppendLine("Visible Objects:");

        if (observation.VisibleObjects.Count == 0)
        {
            builder.AppendLine("- None");
        }
        else
        {
            foreach (var visibleObject in observation.VisibleObjects)
            {
                builder.AppendLine($"- {visibleObject.DisplayName} ({visibleObject.ObjectId}) at {visibleObject.LocationId}, state: {visibleObject.State}");
            }
        }

        builder.AppendLine();
    }

    private static void AppendRetrievedMemories(StringBuilder builder, IReadOnlyList<MemoryRetrievalItem> retrievedMemories)
    {
        builder.AppendLine("# Relevant Memories");

        if (retrievedMemories.Count == 0)
        {
            builder.AppendLine("- None");
        }
        else
        {
            foreach (var item in retrievedMemories)
            {
                builder.AppendLine($"- [{item.Memory.Type}] {item.Memory.Content} (gameTime: {item.Memory.GameTime}, importance: {item.Memory.Importance}, score: {item.FinalScore:0.0000})");
            }
        }

        builder.AppendLine();
    }

    private static void AppendActionInstructions(StringBuilder builder)
    {
        builder.AppendLine("# Available Action Types");

        builder.AppendLine("- MoveTo: Move to a known location. targetId must be a location id.");
        builder.AppendLine("- Wait: Wait for a short time. targetId should be empty.");
        builder.AppendLine("- UseObject: Use a visible object. targetId must be an object id.");
        builder.AppendLine("- TalkTo: Talk to another agent. targetId must be another agent id.");

        builder.AppendLine();

        builder.AppendLine("# Response Format");

        builder.AppendLine("Return only one JSON object with this shape:");

        builder.AppendLine(
            """
            {
                "actionType": "MoveTo | Wait | UseObject | TalkTo",
                "targetId": "",
                "dialogue: "",
                "durationMinutes": 0,
                "reason": ""
            }
            """
        );
    }
}