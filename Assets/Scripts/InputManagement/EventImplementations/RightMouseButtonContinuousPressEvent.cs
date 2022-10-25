using Events;
using UnityEngine;

namespace InputManagement.EventImplementations
{
    public class RightMouseButtonContinuousPressEvent : Event<RightMouseButtonContinuousPressEvent>
    {
        public Vector3 MousePosition;
        public static RightMouseButtonContinuousPressEvent Get(Vector3 mousePos)
        {
            var evt = GetPooledInternal();
            evt.MousePosition = mousePos;
            return evt;
        }
        
    }
}