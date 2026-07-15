using System;

namespace GenerativeNpc.AI
{
    [Serializable]
    public class ObservationResponseDto
    {
        public bool ok;
        public string message;
        public string agentId;
        public int observationCount;
        public string observationId;
        public string receivedAtUtc;

        public bool memoryCreated;
        public string memoryId;
        public int memoryCount;
        public string memoryMessage;

        public int memoryImportance;
        public string memoryImportanceReason;
    }
}