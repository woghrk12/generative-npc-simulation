using System.Collections.Generic;
using UnityEngine;
using GenerativeNpc.AI;
using GenerativeNpc.Simulation;
using GenerativeNpc.World.Object;

namespace GenerativeNpc.World.Agent
{
    public class AgentObservationBuilder : MonoBehaviour
    {
        #region Variables

        [SerializeField] private SimulationClock simulationClock;

        private AgentController agentController;
        private AgentPerception agentPerception;

        #endregion

        #region Unity Events

        private void Awake()
        {
            agentController = GetComponent<AgentController>();
            agentPerception = GetComponent<AgentPerception>();
        }

        #endregion 

        #region Methods

        public AgentObservationDto BuildObservation()
        {
            if (agentController == null)
            {
                Debug.LogError("AgentController is not assigned.");
                return null;
            }

            if (agentPerception == null)
            {
                Debug.LogError("AgentPerception is not assigned.");
                return null;
            }

            if (simulationClock == null)
            {
                Debug.LogError("SimulationClock is not assigned.");
                return null;
            }

            AgentRuntimeState state = agentController.RuntimeState;
            List<WorldObjectSnapshot> visibleObjectSnapshots = agentPerception.ObserveVisibleObjects();

            var observation = new AgentObservationDto
            {
                agentId = agentController.AgentId,
                agentName = agentController.AgentName,

                gameTime = simulationClock.CurrentTimeText,
                currentLocationId = state.currentLocationId,
                status = state.status,

                currentActionType = state.currentActionType,
                currentTargetId = state.currentTargetId,
                currentActionReason = state.currentActionReason,

                lastActionType = state.lastActionType,
                lastTargetId = state.lastTargetId,
                lastActionReason = state.lastActionReason,
            };

            foreach (WorldObjectSnapshot snapshot in visibleObjectSnapshots)
            {
                observation.visibleObjects.Add(new VisibleObjectDto(snapshot));
            }

            return observation;
        }

        public string BuildObservationMemoryText()
        {

            AgentObservationDto observation = BuildObservation();

            if (observation == null)
            {
                return "Observation failed.";
            }

            if (observation.visibleObjects == null || observation.visibleObjects.Count == 0)
            {
                return $"{observation.gameTime}: {observation.agentName} is at {observation.currentLocationId}. No visible objects.";
            }

            List<string> objectTexts = new();

            foreach (VisibleObjectDto visibleObject in observation.visibleObjects)
            {
                objectTexts.Add($"{visibleObject.displayName} ({visibleObject.objectId} is {visibleObject.state}");
            }

            return
                $"{observation.gameTime}: {observation.agentName} is at {observation.currentLocationId}." +
                $"Visible objects: {string.Join(", ", objectTexts)}";
        }

        #endregion
    }
}