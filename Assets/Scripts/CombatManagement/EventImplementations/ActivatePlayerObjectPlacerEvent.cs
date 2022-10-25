using Events;
using Promises;
using UnityEngine;

namespace CombatManagement.EventImplementations
{
    public class ActivatePlayerObjectPlacerEvent : Event<ActivatePlayerObjectPlacerEvent>
    {
        public float MaxDistance;
        public GameObject ObjectToBePlaced;

        public Promise<bool> PlacedPromise;

        public static ActivatePlayerObjectPlacerEvent Get(float maxDist, GameObject objectToThrow)
        {
            var evt = GetPooledInternal();

            evt.MaxDistance = maxDist;
            evt.ObjectToBePlaced = objectToThrow;
            evt.PlacedPromise = Promise<bool>.Create();
            
            return evt;
        }
    }
}