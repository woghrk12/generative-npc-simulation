using System.Collections.Generic;
using UnityEngine;

namespace GenerativeNpc.World 
{
    public class LocationRegistry : MonoBehaviour
    {
        #region Variables

        private readonly Dictionary<string, LocationNode> locations = new();

        #endregion

        #region Unity Events

        private void Awake()
        {
            RegisterAllLocations(); 
        }

        #endregion

        #region Methods

        private void RegisterAllLocations() 
        { 
            locations.Clear();

            var locationNodes = FindObjectsByType<LocationNode>(FindObjectsSortMode.None);
            
            foreach (var node in locationNodes)
            {
                if (string.IsNullOrWhiteSpace(node.LocationId)) 
                {
                    Debug.LogWarning($"LocationNode has empty locationId: {node.name}");
                    continue;
                }

                if (locations.ContainsKey(node.LocationId))
                {
                    Debug.LogWarning($"Duplicated locationId detected: {node.LocationId}");
                    continue;
                }

                locations.Add(node.LocationId, node);
            }

            Debug.Log($"LocationRegistry initialized. Count: {locations.Count}");
        }

        public bool TryGetLocation(string locationId, out LocationNode node) => locations.TryGetValue(locationId, out node);

        #endregion 
    }
}

