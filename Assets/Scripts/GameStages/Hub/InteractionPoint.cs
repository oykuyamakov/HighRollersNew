using UnityEngine;

namespace GameStages.Hub
{
    [RequireComponent(typeof(Collider))]
    public abstract class InteractionPoint : MonoBehaviour
    {
        public bool Entered;

        public abstract void OnTriggerEnter(Collider other);

        public abstract void OnTriggerExit(Collider other);
        
        protected void Reset()
        {
            Entered = false;
        }
    }
}