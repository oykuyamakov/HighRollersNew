using Events;
using UnityEngine;

namespace InputManagement.EventImplementations
{
    public class FireButtonReleasedEvent : Event<FireButtonReleasedEvent>
    {
        public Vector3 m_AimLoc;

        public static FireButtonReleasedEvent Get(Vector3 aimLocation)
        {
            var evt = GetPooledInternal();
            evt.m_AimLoc = aimLocation;
            return evt;
        }
    }
}