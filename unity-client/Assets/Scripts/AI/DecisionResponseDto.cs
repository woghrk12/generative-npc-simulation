using GenerativeNpc.World.Agent;
using System;

namespace GenerativeNpc.AI 
{
    [Serializable]
    public class DecisionResponseDto
    {
        public bool ok;
        public string agentId;

        public string observationId;
        public int observationCount;

        public bool memoryCreated;
        public string memoryId;
        public int memoryCount;
        public string memoryMessage;

        public string decisionQuery;

        public AgentActionDto action;
        public int retrievedMemoryCount;
    }
}