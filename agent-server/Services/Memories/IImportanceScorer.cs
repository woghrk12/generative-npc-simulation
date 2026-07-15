using AgentServer.Contracts.Observations;
using AgentServer.Models.Memories;

namespace AgentServer.Services.Memories;

public interface IImportanceScorer
{
    ImportanceScoreResult Score(AgentObservationRequest observation, string memoryContent);
}