using System;
using UnityEngine;

namespace GenerativeNpc.Simulation 
{
    public class SimulationClock : MonoBehaviour
    {
        #region Variables

        [Header("Initial Time")]
        [SerializeField] private int startDay = 1;
        [SerializeField] private int startHour = 8;
        [SerializeField] private int startMinute = 0;

        [Header("Time Scale")]
        [SerializeField] private float secondsPerGameMinute = 0.1f;

        private int currentTotalMinutes = 0;
        private float elapsedRealSeconds = 0;

        private event Action<int> onMinuteChanged;

        #endregion

        #region Properties

        public event Action<int> OnMinuteChanged 
        { 
            add { onMinuteChanged += value; } 
            remove { onMinuteChanged -= value; }
        }

        public int CurrentMinute => currentTotalMinutes % 60;
        public int CurrentHour => currentTotalMinutes % (24 * 60) / 60;
        public int CurrentDay => startDay + currentTotalMinutes / (24 * 60);

        public float SecondsPerGameMinute => secondsPerGameMinute;

        public string CurrentTimeText => $"Day {CurrentDay}, {CurrentHour:00}:{CurrentMinute:00}";

        #endregion

        #region Unity Events

        private void Awake()
        {
            currentTotalMinutes = startHour * 60 + startMinute;
        }

        private void Update()
        {
            elapsedRealSeconds += Time.deltaTime;

            while (elapsedRealSeconds >= secondsPerGameMinute)
            {
                elapsedRealSeconds -= secondsPerGameMinute;

                currentTotalMinutes++;
                onMinuteChanged?.Invoke(currentTotalMinutes);
            }
        }

        #endregion

        #region Methods

        public float ConvertGameMinutesToSeconds(int gameMinutes) => Mathf.Max(0, gameMinutes) * secondsPerGameMinute;

        #endregion
    }
}
