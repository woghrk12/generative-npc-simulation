using GenerativeNpc.World.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GenerativeNpc.World.Agent 
{
    public class AgentPerception : MonoBehaviour
    {
        #region Variables

        private WorldObjectRegistry worldObjectRegistry;

        private AgentController agentController;

        #endregion

        #region Unity Events

        private void Awake()
        {
            worldObjectRegistry = FindObjectOfType<WorldObjectRegistry>();

            agentController = GetComponent<AgentController>();  
        }

        #endregion

        #region Methods

        public List<WorldObjectSnapshot> ObserveVisibleObjects()
        { 
            var snapshots = new List<WorldObjectSnapshot>();

            if (agentController == null) 
            {
                Debug.LogError("AgentController is not assigned.");
                return snapshots;
            }

            if (worldObjectRegistry == null)
            {
                Debug.LogError("WorldObjectRegistry is not assigned.");
                return snapshots;
            }

            var currentLocationId = agentController.RuntimeState.currentLocationId;
            var worldObjects = worldObjectRegistry.GetObjectsAtLocation(currentLocationId);

            foreach (var worldObject in worldObjects)
            {
                if (worldObject.IsObservable == false) continue;

                snapshots.Add(worldObject.CreateSnapshot());
            }

            return snapshots;   
        }

        public string BuildObservationText() 
        {
            if (agentController == null)
            {
                return "Observation failed because AgentController is not assigned";
            }

            var currentLocationId = agentController.RuntimeState.currentLocationId;
            var visibleObjects = ObserveVisibleObjects();

            if (visibleObjects.Count == 0)
            { 
                return $"{agentController.AgentName} is at {currentLocationId}. No visible objects.";
            }

            var objectTexts = new List<string>();

            foreach (var worldObject in visibleObjects)
            {
                objectTexts.Add(worldObject.ToObservationText());
            }

            return $"{agentController.AgentName} is at {currentLocationId}. Visible object: {string.Join(", ", objectTexts)}.";
        }

        #endregion
    }
}


