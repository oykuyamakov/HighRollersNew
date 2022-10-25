using Events;
using UnityEngine;

namespace CharImplementations.PlayerImplementation.EventImplementations
{
    public class GetPlayerTransformEvent : Event<GetPlayerTransformEvent>
    {
        public Transform playerT;

        public static GetPlayerTransformEvent Get()
        {
            var evt = GetPooledInternal();
            return evt;
        }
    }
}
