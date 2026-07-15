using AgentServer.Models.Memories;

namespace AgentServer.Services.Memories;

public interface IMemoryRetrievalService
{
    IReadOnlyList<MemoryRetrievalItem> Retrieve(string agentId, string query, int topK);
}