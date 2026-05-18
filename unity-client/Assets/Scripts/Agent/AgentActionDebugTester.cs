using System.Collections;
using UnityEngine;

namespace GenerativeNpc.World.Agent 
{
    public class AgentActionDebugTester : MonoBehaviour
    {
        [SerializeField] private AgentActionExecutor agentActionExecutor;
        [SerializeField] private float startDelaySeconds = 1f;

        private IEnumerator Start() 
        {
            yield return new WaitForSeconds(startDelaySeconds);

            if (agentActionExecutor == null)
            {
                Debug.LogError("AgentActionExecutor is not assigned.");
                yield break;
            }

            var moveToCafeAction = new AgentActionDto
            {
                actionType = "MoveTo",
                targetId = "town.cafe",
                dialogue = "",
                durationMinutes = 0,
                reason = "John is going to the cafe for action execution testing."
            };

            yield return agentActionExecutor.Execute(moveToCafeAction);

            var waitAction = new AgentActionDto
            {
                actionType = "Wait",
                targetId = "",
                dialogue = "",
                durationMinutes = 1,
                reason = "John is waiting after arriving at the cafe."
            };

            yield return agentActionExecutor.Execute(waitAction);

            var talkAction = new AgentActionDto
            { 
                actionType = "TalkTo",
                targetId = "maria",
                dialogue = "Hi Maria, how are you today?",
                durationMinutes = 1,
                reason = "John wants to greet Maria."
            };

            yield return agentActionExecutor.Execute(talkAction);
        }
    }
}

