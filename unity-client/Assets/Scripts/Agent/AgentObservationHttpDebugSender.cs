using GenerativeNpc.AI;
using GenerativeNpc.World.Agent;
using System.Collections;
using UnityEngine;

namespace GenerativeNpc.Agent
{
    public class AgentObservationHttpDebugSender
        : MonoBehaviour
    {
        [SerializeField]
        private AgentObservationBuilder observationBuilder;

        [SerializeField]
        private AgentApiClient agentApiClient;

        [SerializeField]
        private float startDelaySeconds = 1f;

        [SerializeField]
        private float sendIntervalSeconds = 3f;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(startDelaySeconds);

            while (true)
            {
                if (observationBuilder == null)
                {
                    Debug.LogError("AgentObservationBuilder is not assigned.");
                    yield break;
                }

                if (agentApiClient == null)
                {
                    Debug.LogError("AgentApiClient is not assigned.");
                    yield break;
                }

                var observation = observationBuilder.BuildObservation();

                if (observation == null)
                {
                    Debug.LogWarning("Observation could not be created.");
                }
                else
                {
                    yield return agentApiClient.SendObservation(observation, HandleSuccess, HandleError);
                }

                yield return new WaitForSeconds(sendIntervalSeconds);
            }
        }

        private void HandleSuccess(ObservationResponseDto response)
        {
            var memoryResult = response.memoryCreated ? $"Memory created: {response.memoryId}" : $"Memory skipped: {response.memoryMessage}";

            Debug.Log(
                $"Observation sent successfully. " +
                $"Agent: {response.agentId}, " +
                $"Observation Count: " +
                $"{response.observationCount}, " +
                $"Memory Count: {response.memoryCount}, " +
                $"{memoryResult}"
            );
        }

        private void HandleError(string errorMessage)
        {
            Debug.LogError(errorMessage);
        }
    }
}