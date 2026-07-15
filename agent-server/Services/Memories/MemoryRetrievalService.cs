using System.Text;
using AgentServer.Models.Memories;

namespace AgentServer.Services.Memories;

public sealed class MemoryRetrievalService : IMemoryRetrievalService
{
    #region Variables

    private const double RECENCY_DECAY_FACTOR = 0.995;

    private const double RELEVANCE_WEIGHT = 0.45;
    private const double IMPORTANCE_WEIGHT = 0.35;
    private const double RECENCY_WEIGHT = 0.20;

    private const int MIN_TOP_K = 1;
    private const int MAX_TOP_K = 20;

    private IMemoryStreamService memoryStreamService;

    #endregion

    public MemoryRetrievalService(IMemoryStreamService memoryStreamService)
    {
        this.memoryStreamService = memoryStreamService;
    }

    public IReadOnlyList<MemoryRetrievalItem> Retrieve(string agentId, string query, int topK)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(agentId);
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var memoryStream = memoryStreamService.GetByAgentId(agentId);

        if (memoryStream.Count == 0)
        {
            return [];
        }

        var normalizedTopK = Math.Clamp(topK, MIN_TOP_K, MAX_TOP_K);
        var now = DateTimeOffset.UtcNow;
        var queryTokens = Tokenize(query);

        var scoredItems = memoryStream
            .Select(item => ScoreMemory(item, queryTokens, now))
            .OrderByDescending(item => item.FinalScore)
            .ThenByDescending(item => item.Memory.CreatedAtUtc)
            .Take(normalizedTopK)
            .ToArray();

        var accessedMemoryIds = scoredItems.Select(item => item.Memory.Id).ToArray();

        memoryStreamService.UpdateLastAccessed(agentId, accessedMemoryIds, now);

        return scoredItems.Select(item => item with { Memory = item.Memory with { LastAccessAtUtc = now } }).ToArray();
    }

    private static MemoryRetrievalItem ScoreMemory(MemoryRecord memory, IReadOnlySet<string> queryTokens, DateTimeOffset now)
    {
        var recencyScore = CalculateRecencyScore(memory, now);
        var importanceScore = CalculateImportanceScore(memory);
        var relevanceScore = CalculateRelevanceScore(memory, queryTokens);

        var finalScore = relevanceScore * RELEVANCE_WEIGHT + importanceScore * IMPORTANCE_WEIGHT + relevanceScore * RELEVANCE_WEIGHT;

        return new MemoryRetrievalItem(Memory: memory, RecencyScore: recencyScore, ImportanceScore: importanceScore, RelevanceScore: relevanceScore, FinalScore: finalScore);
    }

    private static double CalculateRecencyScore(MemoryRecord memory, DateTimeOffset now)
    {
        var elapsedHours = Math.Max(0, (now - memory.LastAccessAtUtc).TotalHours);

        return Math.Pow(RECENCY_DECAY_FACTOR, elapsedHours);
    }

    private static double CalculateImportanceScore(MemoryRecord memory) => Math.Clamp(memory.Importance / 10.0, 0.0, 1.0);

    private static double CalculateRelevanceScore(MemoryRecord memory, IReadOnlySet<string> queryTokens)
    {
        if (queryTokens.Count == 0)
        {
            return 0;
        }

        var memoryTokens = Tokenize(memory.Content);

        if (memoryTokens.Count == 0)
        {
            return 0;
        }

        var matchedCount = queryTokens.Count(memoryTokens.Contains);

        return matchedCount / (double)queryTokens.Count;
    }

    private static HashSet<string> Tokenize(string text)
    {
        var tokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(text))
        {
            return tokens;
        }

        var builder = new StringBuilder();

        foreach (var character in text)
        {
            if (char.IsLetterOrDigit(character) || character == '.')
            {
                builder.Append(char.ToLowerInvariant(character));
            }
            else
            {
                AddTokenIfValid(tokens, builder);
            }
        }

        AddTokenIfValid(tokens, builder);

        return tokens;
    }

    private static void AddTokenIfValid(HashSet<string> tokens, StringBuilder builder)
    {
        if (builder.Length == 0)
        {
            return;
        }

        var token = builder.ToString();

        builder.Clear();

        if (IsStopWord(token))
        {
            return;
        }

        tokens.Add(token);
    }

    private static bool IsStopWord(string token)
    {
        return token is
            "a" or
            "an" or
            "the" or
            "is" or
            "are" or
            "at" or
            "to" or
            "of" or
            "and" or
            "or" or
            "current" or
            "status" or
            "visible" or
            "objects";
    }
}