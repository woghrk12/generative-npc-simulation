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

        private AgentActionExecutor actionExecutor;

        private readonly Queue<AgentActionDto> actionQueue = new();
        private bool isRunning;

        #endregion

        #region Properties

        public string AgentId => agentId;
        public string AgentName => agentName;

        #endregion Properties

        #region Unity Events

        private void Awake()
        {
            actionExecutor = GetComponent<AgentActionExecutor>();

            // Example actions for debugging
            EnqueueAction(new AgentActionDto
            {
                actionType = "MoveTo",
                targetId = "town.cafe",
                dialogue = "",
                durationMinutes = 30,
                reason = "John goes to the cafe as part of his initial test routine",
            });

            EnqueueAction(new AgentActionDto
            {
                actionType = "Wait",
                targetId = "",
                dialogue = "",
                durationMinutes = 10,
                reason = "John waits at the cafe",
            });

            EnqueueAction(new AgentActionDto
            {
                actionType = "MoveTo",
                targetId = "town.park",
                dialogue = "",
                durationMinutes = 30,
                reason = "John goes to the park after visiting the cafe.",
            });

            EnqueueAction(new AgentActionDto
            {
                actionType = "TalkTo",
                targetId = "maria",
                dialogue = "I am testing my daily routine.",
                durationMinutes = 5,
                reason = "John says a test dialogue."
            });

            EnqueueAction(new AgentActionDto
            {
                actionType = "MoveTo",
                targetId = "town.house",
                dialogue = "",
                durationMinutes = 30,
                reason = "John returns home after the test routine."
            });
        }

        private void Start()
        {
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

            Debug.Log($"Agent loop started. AgentId: {AgentId}, Name: {AgentName}");

            while (isRunning)
            {
                if (actionQueue.Count == 0)
                {
                    EnqueueAction(CreateFallbackWaitAction());
                }

                AgentActionDto nextAction = actionQueue.Dequeue();

                Debug.Log($"[{AgentName}] Next action: {nextAction.actionType}\nTarget: {nextAction.targetId}\nReason: {nextAction.reason}");

                yield return actionExecutor.Execute(nextAction);
            }
        }

        private AgentActionDto CreateFallbackWaitAction() 
        {
            return new AgentActionDto
            {
                actionType = "Wait",
                targetId = "",
                dialogue = "",
                durationMinutes = 10,
                reason = "No action is currently queued, so the agent waits"
            };
        }

        #endregion
    }
}
