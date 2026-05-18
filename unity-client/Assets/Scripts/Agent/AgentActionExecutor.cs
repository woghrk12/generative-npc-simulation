using System.Collections;
using UnityEngine;

namespace GenerativeNpc.World.Agent 
{
    public class AgentActionExecutor : MonoBehaviour
    {
        #region Variables

        private LocationRegistry locationRegistry = null;

        private AgentMover agentMover = null;

        #endregion

        #region Properties

        public bool IsExecuting { private set; get; }

        #endregion

        #region Unity Events

        private void Awake()
        {
            locationRegistry = FindObjectOfType<LocationRegistry>();

            agentMover = GetComponent<AgentMover>();    
        }

        #endregion

        #region Methods

        public IEnumerator Execute(AgentActionDto action) 
        {
            if (action == null)
            {
                Debug.LogWarning("Action is null.");
                yield break;
            }

            if (IsExecuting)
            {
                Debug.LogWarning("Agent is already executing an action.");
                yield break;
            }

            IsExecuting = true;

            Debug.Log($"Action started: {action.actionType}, Reason: {action.reason}");

            switch (action.actionType)
            {
                case "MoveTo":
                    yield return ExecuteMoveTo(action);
                    break;

                case "Wait":
                    yield return ExecuteWait(action);
                    break;

                case "TalkTo":
                    yield return ExecuteTalkTo(action);
                    break;

                case "UseObject":
                    yield return ExecuteUseObject(action);
                    break;

                default:
                    Debug.LogWarning($"Unknown action type: {action.actionType}");
                    break;
            }

            Debug.Log($"Action completed: {action.actionType}");

            IsExecuting = false;    
        }

        private IEnumerator ExecuteMoveTo(AgentActionDto action) 
        {
            if (string.IsNullOrWhiteSpace(action.targetId)) 
            {
                Debug.LogWarning("MoveTo action requires targetId.");
                yield break;
            }

            if (locationRegistry == null)
            {
                Debug.LogError("LocationRegistry is not assigned");
                yield break;
            }

            if (agentMover == null)
            {
                Debug.LogError("AgentMover is not assigned");
                yield break;
            }

            if (locationRegistry.TryGetLocation(action.targetId, out LocationNode targetLocation) == false) 
            {
                Debug.LogWarning($"Location is not found: {action.targetId}");
                yield break;
            }

            yield return agentMover.MoveTo(targetLocation.Position);
        }

        private IEnumerator ExecuteWait(AgentActionDto action) 
        {
            Debug.Log($"Waiting for {action.durationMinutes} minutes.");

            yield return new WaitForSeconds(action.durationMinutes);
        }

        private IEnumerator ExecuteTalkTo(AgentActionDto action) 
        {
            if (string.IsNullOrWhiteSpace(action.dialogue))
            {
                Debug.LogWarning("TalkTo action has empty dialogue.");
                yield break;    
            }

            Debug.Log($"Dialogue: {action.dialogue}");

            yield return new WaitForSeconds(action.durationMinutes);
        }

        private IEnumerator ExecuteUseObject(AgentActionDto action) 
        {
            if (string.IsNullOrWhiteSpace(action.targetId))
            {
                Debug.LogWarning("UseObject action requires targetId.");
                yield break;
            }

            Debug.Log($"Use object: {action.targetId}");

            yield return new WaitForSeconds(action.durationMinutes);
        }

        #endregion
    }
}

