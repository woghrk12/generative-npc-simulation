using System;
using GenerativeNpc.World.Object;

namespace GenerativeNpc.AI
{
    [Serializable]
    public class VisibleObjectDto
    {
        public string objectId;
        public string displayName;
        public string locationId;
        public string state;

        public VisibleObjectDto(WorldObjectSnapshot snapshot)
        { 
            objectId = snapshot.objectId;
            displayName = snapshot.displayName;
            locationId = snapshot.locationId;
            state = snapshot.state;
        }
    }
}
