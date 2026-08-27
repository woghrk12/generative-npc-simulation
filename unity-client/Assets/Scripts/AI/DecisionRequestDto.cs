using System;

namespace GenerativeNpc.AI
{
    [Serializable]
    public class DecisionRequestDto
    {
        public AgentObservationDto observation;
        public int memoryTopK;
    }
}