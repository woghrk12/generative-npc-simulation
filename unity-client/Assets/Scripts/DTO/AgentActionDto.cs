using System;

namespace GenerativeNpc.World.Agent 
{
    [Serializable]
    public class AgentActionDto 
    {
        public string actionType;
        public string targetId;
        public string dialogue;
        public int durationMinutes;
        public string reason;
    }
}