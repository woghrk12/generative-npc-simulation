using System;
using UnityEngine;

namespace GenerativeNpc.World.Object 
{
    [Serializable]
    public class WorldObjectState 
    {
        [SerializeField] private string stateText = "normal";

        public string StateText 
        {
            set { stateText = string.IsNullOrWhiteSpace(value) == false ? value.Trim() : "unknown"; }
            get => stateText; 
        }
    }
}
