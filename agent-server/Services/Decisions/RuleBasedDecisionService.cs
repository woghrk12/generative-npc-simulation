using System.Text;
using AgentServer.Contracts.Observations;
using AgentServer.Models.Decisions;
using AgentServer.Models.Memories;
using AgentServer.Services.Memories;

namespace AgentServer.Services.Decisions;

public sealed class RuleBasedDecisionService : IDecisionService
{
    private const int DEFAULT_MEMORY_TOP_K = 5;

    private readonly IMemoryRetrievalService memoryRetrievalService;

    public RuleBasedDecisionService(IMemoryRetrievalService memoryRetrievalService)
    {
        this.memoryRetrievalService = memoryRetrievalService;
    }

    public AgentDecisionResult Decide(AgentObservationRequest observation, int memoryTopK)
    {
        ArgumentNullException.ThrowIfNull(observation);

        var normalizedTopK = memoryTopK > 0 ? memoryTopK : DEFAULT_MEMORY_TOP_K;
        var decisionQuery = BuildDecisionQuery(observation);
        var retrievedMemories = memoryRetrievalService.Retrieve(observation.AgentId, decisionQuery, normalizedTopK);
        var action = DecideAction(observation, retrievedMemories);

        return new AgentDecisionResult(DecisionQuery: decisionQuery, Action: action, RetrievedMemories: retrievedMemories);
    }

    private static string BuildDecisionQuery(AgentObservationRequest observation)
    {
        var builder = new StringBuilder();

        builder.Append($"{observation.AgentName} is at {observation.CurrentLocationId}.");
        builder.Append($"Current status is {observation.Status}");

        if (string.IsNullOrWhiteSpace(observation.CurrentActionType) == false)
        {
            builder.Append($"Current action is {observation.CurrentActionType} toward {observation.CurrentTargetId}");
        }

        if (string.IsNullOrWhiteSpace(observation.LastActionType) == false)
        {
            builder.Append($"Last action was {observation.LastActionType} toward {observation.LastTargetId}");
        }

        if (observation.VisibleObjects.Count == 0)
        {
            builder.Append("No visible objects");
        }
        else
        {
            builder.Append("Visible objects: ");

            var objectDescriptions = observation.VisibleObjects.Select(item => $"{item.DisplayName} ({item.ObjectId}) state {item.State}");

            builder.Append(string.Join(", ", objectDescriptions));
        }

        return builder.ToString();
    }

    private static AgentActionDecision DecideAction(AgentObservationRequest observation, IReadOnlyList<MemoryRetrievalItem> retrievedMemories)
    {
        if (IsBusy(observation))
        {
            return new AgentActionDecision(
                ActionType: "Wait",
                TargetId: "",
                Dialogue: "",
                DurationMinutes: 5,
                Reason: "The Agent is already performing an action, so it should wait briefly."
            );
        }

        var urgentObject = FindUrgentObject(observation.VisibleObjects);

        if (urgentObject is not null)
        {
            return new AgentActionDecision(
                ActionType: "UseObject",
                TargetId: urgentObject.ObjectId,
                Dialogue: "",
                DurationMinutes: 5,
                Reason: $"{observation.AgentName} noticed that {urgentObject.DisplayName} is {urgentObject.State}, so the agent should handle it immediately."
            );
        }

        var coffeeObject = FindCoffeeObject(observation.VisibleObjects);

        if (observation.CurrentLocationId == "town.cafe" && coffeeObject is not null)
        {
            return new AgentActionDecision(
                ActionType: "UseObject",
                TargetId: coffeeObject.ObjectId,
                Dialogue: "",
                DurationMinutes: 10,
                Reason: $"{observation.AgentName} is at the cafe and noticed coffee, so the agent decides to use the counter."
            );
        }

        if (ShouldGoToCafe(observation, retrievedMemories))
        {
            return new AgentActionDecision(
                ActionType: "MoveTo",
                TargetId: "town.cafe",
                Dialogue: "",
                DurationMinutes: 0,
                Reason: $"{observation.AgentName} is not at the cafe and has no urgent issue, so the agent decides to go to the cafe."
            );
        }

        if (observation.CurrentLocationId == "town.cafe")
        {
            return new AgentActionDecision(
                ActionType: "MoveTo",
                TargetId: "town.park",
                Dialogue: "",
                DurationMinutes: 0,
                Reason: $"{observation.AgentName} finished checking the cafe and decides to go to the park."
            );
        }

        if (observation.CurrentLocationId == "town.park")
        {
            return new AgentActionDecision(
                ActionType: "MoveTo",
                TargetId: "town.house",
                Dialogue: "",
                DurationMinutes: 0,
                Reason: $"{observation.AgentName} finished walking around the park and decides to return home."
            );
        }

        return new AgentActionDecision(
            ActionType: "Wait",
            TargetId: "",
            Dialogue: "",
            DurationMinutes: 10,
            Reason: "No specific rule matched, so the agent waits."
        );
    }

    private static bool IsBusy(AgentObservationRequest observation)
    {
        if (ContainsAny(observation.Status, "moving", "using", "talking"))
        {
            return true;
        }

        return string.IsNullOrWhiteSpace(observation.CurrentActionType) == false;
    }

    private static VisibleObjectRequest? FindUrgentObject(IReadOnlyList<VisibleObjectRequest> visibleObjects) => visibleObjects.FirstOrDefault(item => ContainsAny(item.State, "burning", "fire", "smoke", "danger", "leaking", "broken", "flooding"));

    private static VisibleObjectRequest? FindCoffeeObject(IReadOnlyList<VisibleObjectRequest> visibleObjects) => visibleObjects.FirstOrDefault(item => ContainsAny(item.State, "coffee") || ContainsAny(item.ObjectId, "counter"));

    private static bool ShouldGoToCafe(AgentObservationRequest observation, IReadOnlyList<MemoryRetrievalItem> retrievedMemories)
    {
        if (observation.CurrentLocationId == "town.cafe")
        {
            return false;
        }

        if (observation.CurrentLocationId == "town.house")
        {
            return true;
        }

        return retrievedMemories.Any(item => ContainsAny(item.Memory.Content, "coffee", "cafe", "counter"));
    }

    private static bool ContainsAny(string? text, params string[] keywords)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        foreach (var keyword in keywords)
        {
            if (text.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}