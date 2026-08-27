using GenerativeNpc.World.Agent;
using GenerativeNpc.World.Object;
using UnityEngine;

namespace GenerativeNpc.World
{
    public class WorldObjectActionEffectResolver : MonoBehaviour
    {
        private WorldObjectRegistry worldObjectRegistry;

        private void Awake()
        {
            worldObjectRegistry = FindObjectOfType<WorldObjectRegistry>();
        }

        public bool TryApplyUseObject(AgentActionDto action, out string effectDescription)
        { 
            effectDescription = string.Empty;

            if (worldObjectRegistry == null)
            {
                effectDescription = "WorldObjectRegistry is not assigned";
                return false;
            }

            if (action == null) 
            {
                effectDescription = "Action is null.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(action.targetId))
            {
                effectDescription = "UseObject targetId is empty.";
                return false;
            }

            if (worldObjectRegistry.TryGetObject(action.targetId, out var worldObject) == false)
            {
                effectDescription = $"World object not found: {action.targetId}";
                return false;
            }

            var beforeState = worldObject.StateText;
            var nextState = ResolveNextState(worldObject.ObjectId, beforeState);

            if (string.Equals(beforeState, nextState, System.StringComparison.OrdinalIgnoreCase))
            {
                effectDescription = $"{worldObject.DisplayName} state remains '{beforeState}'.";
                return false;
            }

            worldObject.StateText = nextState;
            effectDescription = $"{worldObject.DisplayName} state changed from '{beforeState}' to '{nextState}'";

            return true;
        }

        private static string ResolveNextState(string objectId, string currentState)
        {
            var normalizedObjectId = objectId?.ToLowerInvariant() ?? string.Empty;
            var normalizedState = currentState?.ToLowerInvariant() ?? string.Empty;

            if (normalizedObjectId.Contains("counter") && normalizedState.Contains("coffee"))
            {
                return "empty";
            }

            if (normalizedObjectId.Contains("stove") && (normalizedState.Contains("burning") || normalizedState.Contains("fire") || normalizedState.Contains("smoke")))
            {
                return "off";
            }

            if (normalizedState.Contains("leaking"))
            {
                return "fixed";
            }

            if (normalizedState.Contains("broken"))
            {
                return "fixed";
            }

            if (normalizedState.Contains("dirty"))
            {
                return "clean";
            }

            if (normalizedState.Contains("locked"))
            {
                return "unlocked";
            }

            return currentState;
        }
    }
}
