using System;

namespace GenerativeNpc.World.Agent
{
    [Serializable]
    public class AgentRuntimeState
    {
        public string currentLocationId;
        public string status = "Idle";

        public string currentActionType;
        public string currentTargetId;
        public string currentActionReason;

        public string lastActionType;
        public string lastTargetId;
        public string lastActionReason;

        public string lastUpdatedTimeText;

        public void Initialize(string initialLocationId, string timeText)
        { 
            currentLocationId = initialLocationId;
            status = "Idle";

            currentActionType = "";
            currentTargetId = "";
            currentActionReason = "";

            lastActionType = "";
            lastActionReason = "";
            lastActionReason = "";

            lastUpdatedTimeText = timeText;
        }

        public void StartAction(AgentActionDto action, string timeText) 
        {
            status = "Executing";

            currentActionType = action.actionType;
            currentTargetId = action.targetId;
            currentActionReason = action.reason;

            lastUpdatedTimeText = timeText;
        }

        public void CompleteAction(AgentActionDto action, string timeText)
        {
            status = "Idle";

            lastActionType = action.actionType;
            lastTargetId = action.targetId;
            lastActionReason = action.reason;

            if (action.actionType == "MoveTo" && string.IsNullOrWhiteSpace(action.targetId) == false)
            {
                currentLocationId = action.targetId;
            }

            currentActionType = "";
            currentTargetId = "";
            currentActionType = "";

            lastUpdatedTimeText = timeText; 
        }

        public void SetWaitingForAction(string reason, string timeText) 
        {
            status = "WaitingForAction";

            currentActionType = string.Empty;
            currentTargetId = string.Empty;
            currentActionReason = string.Empty; 

            lastUpdatedTimeText = timeText; 
        }
    }
}