using System.Collections;
using UnityEngine;

namespace GenerativeNpc.World.Agent 
{
    public class AgentMover : MonoBehaviour
    {
        #region Variables

        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float arriveDistance = 0.05f;

        #endregion

        #region Properties

        public bool IsMoveing { private set; get; }

        #endregion Properties

        #region Methods

        public IEnumerator MoveTo(Vector3 destination) 
        {
            if (IsMoveing) 
            {
                yield break;
            }

            IsMoveing = true;

            while (Vector3.Distance(transform.position, destination) > arriveDistance) 
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

                yield return null;
            }

            transform.position = destination;
            IsMoveing = false;
        }

        #endregion Methods
    }
}
