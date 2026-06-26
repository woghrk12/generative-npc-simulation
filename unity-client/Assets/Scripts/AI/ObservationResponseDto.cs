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
    }
}