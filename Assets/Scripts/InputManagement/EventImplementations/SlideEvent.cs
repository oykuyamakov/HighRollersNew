using Events;
using UnityEngine;

namespace InputManagement.EventImplementations
{
    public class SlideEvent : Event<SlideEvent>
    {
        public static SlideEvent Get()
        {
            var evt = GetPooledInternal();
            return evt;
        }
    }
    
}
