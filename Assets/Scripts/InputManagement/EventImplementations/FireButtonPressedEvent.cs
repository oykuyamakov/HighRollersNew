using Events;
using UnityEngine;

namespace InputManagement.EventImplementations
{
    public class FireButtonPressedEvent : Event<FireButtonPressedEvent>
    {
        public Vector3 m_AimLoc;
        
        public static FireButtonPressedEvent Get(Vector3 aimLocation)
        {
            var evt = GetPooledInternal();
            evt.m_AimLoc = aimLocation;
            return evt;
        }
    }
}
