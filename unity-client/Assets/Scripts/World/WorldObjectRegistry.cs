using System.Collections.Generic;
using UnityEngine;

namespace GenerativeNpc.World.Object 
{
    public class WorldObjectRegistry : MonoBehaviour
    {
        #region Variables

        private readonly Dictionary<string, WorldObject> objectsById = new();
        private readonly Dictionary<string, List<WorldObject>> objectsByLocationId = new();

        #endregion

        #region Unity Events

        private void Awake()
        {
            RegisterAllObjects();
        }

        #endregion

        #region Methods

        private void RegisterAllObjects()
        {
            objectsById.Clear();
            objectsByLocationId.Clear();

            var worldObjects = FindObjectsByType<WorldObject>(FindObjectsSortMode.None);

            foreach (var worldObject in worldObjects)
            {
                if (string.IsNullOrWhiteSpace(worldObject.ObjectId))
                {
                    Debug.LogWarning($"WorldObject has empty objectId: {worldObject.name}");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(worldObject.LocationId))
                {
                    Debug.LogWarning($"WorldObject has empty locationId: {worldObject.name}");
                    continue;
                }

                if (objectsById.ContainsKey(worldObject.ObjectId) == true)
                {
                    Debug.LogWarning($"Duplicated objectId detected: {worldObject.name}");
                    continue;
                }

                objectsById.Add(worldObject.ObjectId, worldObject);

                if (objectsByLocationId.ContainsKey(worldObject.LocationId) == false)
                {
                    objectsByLocationId.Add(worldObject.LocationId, new List<WorldObject>());
                }

                objectsByLocationId[worldObject.LocationId].Add(worldObject);
            }

            Debug.Log($"WorldObjectRegistry initialized. Count: {objectsById.Count}");
        }

        public bool TryGetObject(string objectId, out WorldObject worldObject) => objectsById.TryGetValue(objectId, out worldObject);

        public List<WorldObject> GetObjectsAtLocation(string locationId) => objectsByLocationId.TryGetValue(locationId, out var worldObjects) == true ? new List<WorldObject>(worldObjects) : new List<WorldObject>();
        

        #endregion 
    }
}
