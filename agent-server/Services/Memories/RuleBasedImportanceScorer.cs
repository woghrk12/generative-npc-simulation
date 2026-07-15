using AgentServer.Contracts.Observations;
using AgentServer.Models.Memories;

namespace AgentServer.Services.Memories;

public sealed class RuleBasedImportanceScorer : IImportanceScorer
{
    public ImportanceScoreResult Score(AgentObservationRequest observation, string memoryContent)
    {
        ArgumentNullException.ThrowIfNull(observation);
        ArgumentNullException.ThrowIfNull(memoryContent);

        var score = 2;
        var reason = "Routine observation";

        ApplyCandidate(ref score, ref reason, candidateScore: ScoreFromStatus(observation.Status), candidateReason: "The agent's current status is notable.");
        ApplyCandidate(ref score, ref reason, candidateScore: ScoreFromAction(observation.CurrentActionType), candidateReason: "The agent is performing a notable action.");
        ApplyCandidate(ref score, ref reason, candidateScore: ScoreFromAction(observation.LastActionType), candidateReason: "The agent recently performed a notable action.");

        foreach (var visibleObject in observation.VisibleObjects)
        {
            var objectScore = ScoreFromObjectState(visibleObject.State);

            ApplyCandidate(ref score, ref reason, candidateScore: objectScore, candidateReason: $"Object '{visibleObject.ObjectId}' has notable state '{visibleObject.State}'.");
        }

        ApplyCandidate(ref score, ref reason, candidateScore: ScoreFromMemoryContent(memoryContent), candidateReason: "The memory content contains notable terms.");

        return new ImportanceScoreResult(ClampScore(score), reason);
    }

    private static int ScoreFromStatus(string? status)
    {
        if (ContainsAny(status, "panic", "danger", "emergency", "injured", "sick"))
        {
            return 9;
        }

        if (ContainsAny(status, "talking", "conversation", "meeting"))
        {
            return 6;
        }

        if (ContainsAny(status, "moving", "using", "working"))
        {
            return 3;
        }

        return 2;
    }

    private static int ScoreFromAction(string? actionType)
    {
        if (ContainsAny(actionType, "TalkTo", "Talk", "Conversation"))
        {
            return 6;
        }

        if (ContainsAny(actionType, "UseObject"))
        {
            return 4;
        }

        if (ContainsAny(actionType, "MoveTo"))
        {
            return 3;
        }

        return 2;
    }

    private static int ScoreFromObjectState(string? objectState)
    {
        if (ContainsAny(objectState, "burning", "fire", "broken", "danger", "leaking", "flooding", "smoke"))
        {
            return 9;
        }

        if (ContainsAny(objectState, "occupied", "missing", "empty", "dirty", "locked"))
        {
            return 4;
        }

        if (ContainsAny(objectState, "has", "changed", "on"))
        {
            return 4;
        }

        if (ContainsAny(objectState, "off", "made", "clean", "normal"))
        {
            return 2;
        }

        return 3;
    }

    private static int ScoreFromMemoryContent(string? memoryContent)
    {
        if (ContainsAny(memoryContent, "burning", "emergency", "danger", "injured", "attack"))
        {
            return 9;
        }

        if (ContainsAny(memoryContent, "party", "invitation", "date", "promise", "argument", "conflict", "secret"))
        {
            return 8;
        }

        if (ContainsAny(memoryContent, "talking", "conversation", "met", "friend"))
        {
            return 6;
        }

        return 2;
    }

    private static void ApplyCandidate(ref int currentScore, ref string currentReason, int candidateScore, string candidateReason)
    {
        if (candidateScore <= currentScore)
        {
            return;
        }

        currentScore = candidateScore;
        currentReason = candidateReason;
    }

    private static bool ContainsAny(string? text, params string[] keywords)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        foreach (string keyword in keywords)
        {
            if (text.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static int ClampScore(int score) => Math.Clamp(score, min: 1, max: 10);
}