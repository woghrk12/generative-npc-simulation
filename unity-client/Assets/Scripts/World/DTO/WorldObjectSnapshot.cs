using System;

namespace GenerativeNpc.World.Object
{
    [Serializable]
    public class WorldObjectSnapshot
    {
        public string objectId;
        public string displayName;
        public string locationId;
        public string state;

        public WorldObjectSnapshot(string objectId, string displayName, string locationId, string state)
        {
            this.objectId = objectId;
            this.displayName = displayName;
            this.locationId = locationId;
            this.state = state;
        }

        public string ToObservationText() => $"{displayName} ({objectId}) is {state}";
    }
}