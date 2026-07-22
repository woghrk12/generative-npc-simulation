using System.Text;
using AgentServer.Models.Memories;

namespace AgentServer.Services.Memories;

public sealed class MemoryRetrievalService : IMemoryRetrievalService
{
    #region Variables

    private const double RECENCY_DECAY_FACTOR = 0.995;

    private const double MIN_MAX_EPSILON = 0.000000001;

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

        var rawItems = memoryStream
            .Select(memory => ScoreMemoryRaw(memory, queryTokens, now))
            .ToArray();

        var scoredItems = ApplyMinMaxScaling(rawItems)
            .OrderByDescending(item => item.FinalScore)
            .ThenByDescending(item => item.Memory.CreatedAtUtc)
            .Take(normalizedTopK)
            .ToArray();

        var accessedMemoryIds = scoredItems.Select(item => item.Memory.Id).ToArray();

        memoryStreamService.UpdateLastAccessed(agentId, accessedMemoryIds, now);

        return scoredItems.Select(item => item with { Memory = item.Memory with { LastAccessAtUtc = now } }).ToArray();
    }

    private static RawMemoryRetrievalItem ScoreMemoryRaw(MemoryRecord memory, IReadOnlySet<string> queryTokens, DateTimeOffset now)
    {
        var rawRecencyScore = CalculateRecencyScore(memory, now);
        var rawImportanceScore = CalculateImportanceScore(memory);
        var rawRelevanceScore = CalculateRelevanceScore(memory, queryTokens);

        return new RawMemoryRetrievalItem(Memory: memory, RawRecencyScore: rawRecencyScore, RawImportanceScore: rawImportanceScore, RawRelevanceScore: rawRelevanceScore);
    }

    private static double CalculateRecencyScore(MemoryRecord memory, DateTimeOffset now)
    {
        var elapsedHours = Math.Max(0, (now - memory.LastAccessAtUtc).TotalHours);

        return Math.Pow(RECENCY_DECAY_FACTOR, elapsedHours);
    }

    private static double CalculateImportanceScore(MemoryRecord memory) => memory.Importance;

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

    private static IReadOnlyList<MemoryRetrievalItem> ApplyMinMaxScaling(IReadOnlyList<RawMemoryRetrievalItem> rawItems)
    {
        if (rawItems.Count == 0)
        {
            return [];
        }

        var (minRecencyScore, maxRecencyScore) = GetRange(rawItems.Select(item => item.RawRecencyScore));
        var (minImportanceScore, maxImportanceScore) = GetRange(rawItems.Select(item => item.RawImportanceScore));
        var (minRelevanceScore, maxRelevanceScore) = GetRange(rawItems.Select(item => item.RawRelevanceScore));

        return rawItems.Select(item =>
        {
            var recencyScore = Scale(item.RawRecencyScore, minRecencyScore, maxRecencyScore);
            var importanceScore = Scale(item.RawImportanceScore, minImportanceScore, maxImportanceScore);
            var relevanceScore = Scale(item.RawRelevanceScore, minRelevanceScore, maxRelevanceScore);

            return new MemoryRetrievalItem(
                Memory: item.Memory,
                RecencyScore: recencyScore,
                ImportanceScore: importanceScore,
                RelevanceScore: relevanceScore,
                FinalScore: recencyScore + importanceScore + relevanceScore
            );
        }).ToArray();
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

    private static (double, double) GetRange(IEnumerable<double> values)
    {
        var min = double.PositiveInfinity;
        var max = double.NegativeInfinity;

        foreach (var value in values)
        {
            min = Math.Min(min, value);
            max = Math.Max(max, value);
        }

        return double.IsPositiveInfinity(min) ? (0, 0) : (min, max);
    }

    private static double Scale(double value, double min, double max)
    {
        var range = max - min;

        if (Math.Abs(range) < MIN_MAX_EPSILON)
        {
            return 0;
        }

        return Math.Clamp((value - min) / range, 0.0, 1.0);
    }
}