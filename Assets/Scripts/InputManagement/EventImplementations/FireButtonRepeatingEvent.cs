using Events;
using UnityEngine;

namespace InputManagement.EventImplementations
{
    public class FireButtonRepeatingEvent : Event<FireButtonRepeatingEvent>
    {
        public Vector3 m_AimLoc;
        
        public static FireButtonRepeatingEvent Get(Vector3 aimLocation)
        {
            var evt = GetPooledInternal();
            evt.m_AimLoc = aimLocation;
            return evt;
        }
    }
}
