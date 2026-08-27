using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace GenerativeNpc.AI
{
    public class AgentApiClient : MonoBehaviour
    {
        #region Variables

        [SerializeField] private string baseUrl = "http://127.0.0.1:4000";
        [SerializeField] private int timeoutSeconds = 10;

        #endregion

        public IEnumerator SendObservation(AgentObservationDto observation, Action<ObservationResponseDto> onSuccess, Action<string> onError) 
        {
            if (observation == null)
            {
                onError?.Invoke("Obvservation is null");
                yield break;
            }

            if (string.IsNullOrWhiteSpace(observation.agentId))
            {
                onError?.Invoke("Observation agentId is empty.");
                yield break;
            }

            var json = JsonUtility.ToJson(observation);

            var escapedAgentId = UnityWebRequest.EscapeURL(observation.agentId);

            var requestUrl = $"{baseUrl.TrimEnd('/')}/agents/{escapedAgentId}/observations";
            var requestBody = Encoding.UTF8.GetBytes(json);

            using (var request = new UnityWebRequest(requestUrl, UnityWebRequest.kHttpVerbPOST))
            {
                request.uploadHandler = new UploadHandlerRaw(requestBody);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.timeout = timeoutSeconds;
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success) {
                    var responseBody = request.downloadHandler?.text ?? "";

                    onError?.Invoke($"Observation request failed. Status: {request.responseCode}, Error: {request.error}, Response: {responseBody}");

                    yield break;
                }

                ObservationResponseDto response;

                try
                {
                    response = JsonUtility.FromJson<ObservationResponseDto>(request.downloadHandler.text);
                }
                catch (Exception exception)
                {
                    onError?.Invoke($"Failed to parse observation. Response: {exception.Message}");
                    yield break;
                }

                if (response == null)
                {
                    onError?.Invoke("Observation response is null.");
                    yield break;
                }

                onSuccess?.Invoke(response);
            }
        }

        public IEnumerator RequestNextAction(AgentObservationDto observation, int memoryTopK, Action<DecisionResponseDto> onSuccess, Action<string> onError)
        {
            if (observation == null)
            {
                onError?.Invoke("Observation is null.");
                yield break;
            }

            if (string.IsNullOrWhiteSpace(observation.agentId))
            {
                onError?.Invoke("Observation agentId is empty.");
                yield break;
            }

            var json = JsonUtility.ToJson(new DecisionRequestDto { observation = observation, memoryTopK = memoryTopK });

            var escapedAgentId = UnityWebRequest.EscapeURL(observation.agentId);

            var requestUrl = $"{baseUrl.TrimEnd('/')}/agents/{escapedAgentId}/decisions/next-action";
            var requestBody = Encoding.UTF8.GetBytes(json);

            using (var request = new UnityWebRequest(requestUrl, UnityWebRequest.kHttpVerbPOST))
            {
                request.uploadHandler = new UploadHandlerRaw(requestBody);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.timeout = timeoutSeconds;
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    var responseBody = request.downloadHandler?.text ?? "";

                    onError?.Invoke($"Decision request failed. Status: {request.responseCode}, Error: {request.error}, Response: {responseBody}");

                    yield break;    
                }

                DecisionResponseDto response;

                try
                {
                    response = JsonUtility.FromJson<DecisionResponseDto>(request.downloadHandler.text);
                }
                catch (Exception exception)
                {
                    onError?.Invoke($"Failed to parse decision response: {exception.Message}");
                    yield break;
                }

                if (response == null)
                {
                    onError?.Invoke("Decision response is null.");
                    yield break;
                }

                if (response.action == null)
                {
                    onError?.Invoke("Decision response action is null.");
                    yield break;
                }

                onSuccess?.Invoke(response);
            }
        }
    }
}