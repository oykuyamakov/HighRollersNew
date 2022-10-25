using Events;
using UnityEngine;

namespace CharImplementations.PlayerImplementation.EventImplementations
{
    public class PlayerDeadEvent : Event<PlayerDeadEvent>
    {
        public static PlayerDeadEvent Get()
        {
            var evt = GetPooledInternal();
            return evt;
        }
    }
}
