using System;
using System.Collections.Generic;

namespace GenerativeNpc.AI
{
    [Serializable]
    public class AgentObservationDto
    {
        public string agentId;
        public string agentName;

        public string gameTime;
        public string currentLocationId;
        public string status;

        public string currentActionType;
        public string currentTargetId;
        public string currentActionReason;

        public string lastActionType;
        public string lastTargetId;
        public string lastActionReason;

        public List<VisibleObjectDto> visibleObjects = new();
    }
}
