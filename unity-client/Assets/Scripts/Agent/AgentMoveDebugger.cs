using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerativeNpc.World.Agent 
{
    public class AgentMoveDebugger : MonoBehaviour
    {
        [SerializeField] private AgentMover agentMover;
        [SerializeField] private LocationRegistry locationRegistry;
        [SerializeField] private string targetLocationId = "town.cafe";
        [SerializeField] private float startDelaySeconds = 1f;

        private IEnumerator Start() 
        {
            yield return new WaitForSeconds(startDelaySeconds);

            if (agentMover == null) {
                Debug.LogError("AgentMover is not assigned");
                yield break;
            }

            if (locationRegistry == null)
            {
                Debug.LogError("LocationRegistry is not assigned");
                yield break;
            }

            if (locationRegistry.TryGetLocation(targetLocationId, out LocationNode targetLocation) == false) 
            {
                Debug.LogWarning($"Location is not found: {targetLocationId}");
                yield break;
            }

            Debug.Log($"Move started. Target: {targetLocation.LocationId}");

            yield return agentMover.MoveTo(targetLocation.Position);

            Debug.Log($"Move completed. Target: {targetLocation.LocationId}");
        }
    }
}

