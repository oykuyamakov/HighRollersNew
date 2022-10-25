using Events;
using UnityEngine;

namespace InputManagement.EventImplementations
{
    public class RightMousePressedEvent : Event<RightMousePressedEvent>
    {
        public Vector3 MousePosition;
        public static RightMousePressedEvent Get(Vector3 mousPos)
        {
            var evt = GetPooledInternal();
            evt.MousePosition = mousPos;
            return evt;
        }
    }
}