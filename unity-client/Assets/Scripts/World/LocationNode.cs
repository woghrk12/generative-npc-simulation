using UnityEngine;

namespace GenerativeNpc.World 
{ 
    public class LocationNode : MonoBehaviour
    {
        #region Variables

        [SerializeField] private string locationId;
        [SerializeField] private string displayName;

        #endregion

        #region Properties

        public string LocationId => locationId;
        public string DisplayName => displayName;

        #endregion
    }
}
