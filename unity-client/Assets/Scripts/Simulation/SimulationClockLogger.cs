using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerativeNpc.Simulation 
{
    public class SimulationClockLogger : MonoBehaviour
    {
        [SerializeField] private SimulationClock clock;
        [SerializeField] private int logIntervalMinutes = 10;

        private void OnEnable()
        {
            if (clock == null)
            {
                Debug.LogWarning("SimulationClock is not assigned.");
                return;
            }

            clock.OnMinuteChanged += HandleMinuteChanged;
        }

        private void OnDisable()
        {
            if (clock == null)
            {
                Debug.LogWarning("SimulationClock is not assigned.");
                return;
            }

            clock.OnMinuteChanged -= HandleMinuteChanged;
        }

        private void HandleMinuteChanged(int currentTotalMinutes)
        {
            if (currentTotalMinutes % logIntervalMinutes != 0)
            {
                return;
            }

            Debug.Log($"Current simulation time: {clock.CurrentTimeText}"); 
        }
    }
}

