using Events;
using UnityEngine;

namespace InputManagement.EventImplementations
{
    public class WeaponWheelEvent : Event<WeaponWheelEvent>
    {
        public static WeaponWheelEvent Get()
        {
            var evt = GetPooledInternal();
            return evt;
        }
    }
}
