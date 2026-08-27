using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerativeNpc.World.Object
{
    public class WorldObject : MonoBehaviour
    {
        #region Variables

        [SerializeField] private string objectId;
        [SerializeField] private string displayName;
        [SerializeField] private string locationId;
        [SerializeField] private bool isObservable = true;
        [SerializeField] private WorldObjectState objectState = new();

        #endregion

        #region Properties

        public string ObjectId => objectId;
        public string DisplayName => displayName;
        public string LocationId => locationId;
        public bool IsObservable => isObservable;
        public string StateText 
        {
            set 
            {
                if (objectState == null) return;

                objectState.StateText = value;
            }
            get => objectState != null ? objectState.StateText : "unknown";
        }
        #endregion

        #region Methods

        public void SetState(string newStateText) 
        {
            objectState ??= new WorldObjectState();
            objectState.StateText = newStateText;
        }

        public WorldObjectSnapshot CreateSnapshot() => new WorldObjectSnapshot(ObjectId, DisplayName, LocationId, StateText);

        #endregion 
    }
}

