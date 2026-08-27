using GenerativeNpc.AI;
using GenerativeNpc.Simulation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerativeNpc.World.Agent 
{
    public class AgentController : MonoBehaviour
    {
        #region Variables

        [Header("Agent Identity")]
        [SerializeField] private string agentId;
        [SerializeField] private string agentName;

        [Header("Initial State")]
        [SerializeField] private string initialLocationId = "town.house";
        [SerializeField] private int memoryTopK = 5;

        [Header("State")]
        [SerializeField] private AgentRuntimeState runtimeState = new();

        private AgentApiClient agentApiClient;
        private SimulationClock simulationClock;

        private AgentActionExecutor actionExecutor;
        private AgentObservationBuilder observationBuilder;

        private readonly Queue<AgentActionDto> actionQueue = new();
        private bool isRunning;

        #endregion

        #region Properties

        public string AgentId => agentId;
        public string AgentName => agentName;

        public AgentRuntimeState RuntimeState => runtimeState;

        #endregion Properties

        #region Unity Events

        private void Awake()
        {
            agentApiClient = FindObjectOfType<AgentApiClient>();
            simulationClock = FindObjectOfType<SimulationClock>();

            actionExecutor = GetComponent<AgentActionExecutor>();
            observationBuilder = GetComponent<AgentObservationBuilder>();

            // Example actions for debugging
            //EnqueueAction(new AgentActionDto
            //{
            //    actionType = "MoveTo",
            //    targetId = "town.cafe",
            //    dialogue = "",
            //    durationMinutes = 30,
            //    reason = "John goes to the cafe as part of his initial test routine",
            //});

            //EnqueueAction(new AgentActionDto
            //{
            //    actionType = "Wait",
            //    targetId = "",
            //    dialogue = "",
            //    durationMinutes = 10,
            //    reason = "John waits at the cafe",
            //});

            //EnqueueAction(new AgentActionDto
            //{
            //    actionType = "MoveTo",
            //    targetId = "town.park",
            //    dialogue = "",
            //    durationMinutes = 30,
            //    reason = "John goes to the park after visiting the cafe.",
            //});

            //EnqueueAction(new AgentActionDto
            //{
            //    actionType = "TalkTo",
            //    targetId = "maria",
            //    dialogue = "I am testing my daily routine.",
            //    durationMinutes = 5,
            //    reason = "John says a test dialogue."
            //});

            //EnqueueAction(new AgentActionDto
            //{
            //    actionType = "MoveTo",
            //    targetId = "town.house",
            //    dialogue = "",
            //    durationMinutes = 30,
            //    reason = "John returns home after the test routine."
            //});
        }

        private void Start()
        {
            runtimeState ??= new AgentRuntimeState();
            runtimeState.Initialize(initialLocationId, simulationClock.CurrentTimeText);

            StartAction();   
        }

        #endregion

        #region Methods

        public void StartAction() 
        {
            isRunning = true;

            StartCoroutine(RunLoop());
        }

        public void StopAction()
        {
            isRunning = false;
        }

        public void EnqueueAction(AgentActionDto action) 
        {
            if (action == null)
            {
                Debug.LogWarning("Cannot enqueue null action.");
                return;
            }

            actionQueue.Enqueue(action);
        }

        private IEnumerator RunLoop()
        {
            if (actionExecutor == null)
            {
                Debug.LogError("AgentActionExecutor is not assigned.");
                yield break;
            }

            Debug.Log(
                $"Agent loop started.\n" +
                $"AgentId: {AgentId}\n" +
                $"Name: {AgentName}\n" +
                $"Initial Location: {runtimeState.currentLocationId}\n" +
                $"Time: {simulationClock.CurrentTimeText}"
            );

            while (isRunning)
            {
                if (actionQueue.Count == 0)
                {
                    runtimeState.SetWaitingForAction("No action is currently queued.", simulationClock.CurrentTimeText);

                    yield return RequestNextActionFromServer();

                    if (actionQueue.Count == 0)
                    {
                        yield return new WaitForSeconds(1f);
                        continue;
                    }
                }

                AgentActionDto nextAction = actionQueue.Dequeue();

                Debug.Log(
                    $"[{AgentName}] Next action: {nextAction.actionType}\n" +
                    $"Target: {nextAction.targetId}\n" +
                    $"Reason: {nextAction.reason}"
                );

                runtimeState.StartAction(nextAction, simulationClock.CurrentTimeText);

                yield return actionExecutor.Execute(nextAction);

                runtimeState.CompleteAction(nextAction, simulationClock.CurrentTimeText);

                Debug.Log(
                    $"[{AgentName}] State updated.\n" +
                    $"Location: {runtimeState.currentLocationId}\n" +
                    $"Status: {runtimeState.status}\n" +
                    $"Last Action: {runtimeState.lastActionType}\n" +
                    $"Time: {runtimeState.lastUpdatedTimeText}"
                );
            }
        }

        private IEnumerator RequestNextActionFromServer()
        {
            if (observationBuilder == null) 
            {
                Debug.LogError("observationBuilder is not assigned.");
                EnqueueFallbackWaitAction("Observation builder is missing.");
                yield break;
            }

            if (agentApiClient == null)
            {
                Debug.LogError("agentApiClient is not assigned.");
                EnqueueFallbackWaitAction("Agent API Client is missing.");
                yield break;
            }

            var observation = observationBuilder.BuildObservation();

            DecisionResponseDto response = null;
            string error = null;

            yield return agentApiClient.RequestNextAction(observation, memoryTopK, onSuccess: result => response = result, onError: message => error = message);

            if (string.IsNullOrWhiteSpace(error) == false)
            {
                Debug.LogError(error);
                EnqueueFallbackWaitAction("Failed to request next action from server.");
                yield break;
            }

            if (response == null || response.action == null)
            {
                Debug.LogError("Decision response or action is null.");
                EnqueueFallbackWaitAction("Decision response was invalid.");
                yield break;
            }

            if (string.IsNullOrWhiteSpace(response.action.actionType))
            {
                Debug.LogError("Decision actionType is empty.");
                EnqueueFallbackWaitAction("Decision action type was empty.");
                yield break;    
            }

            actionQueue.Enqueue(response.action);

            Debug.Log(
                $"[Decision Received] " +
                $"agent={response.agentId}, " +
                $"action={response.action.actionType}, " +
                $"target={response.action.targetId}, " +
                $"reason={response.action.reason}, " +
                $"retrievedMemories={response.retrievedMemoryCount}"
            );
        }

        private void EnqueueFallbackWaitAction(string reason)
        {
            actionQueue.Enqueue(new AgentActionDto 
            { 
                actionType = "Wait",
                targetId = "",
                dialogue = "",
                durationMinutes = 5,
                reason = reason
            });
        }

        #endregion
    }
}
