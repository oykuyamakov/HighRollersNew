using Events;
using UnityEngine;

namespace CharImplementations.PlayerImplementation.EventImplementations
{
    public class PlayerHealthChangeEvent : Event<PlayerHealthChangeEvent>
    {
        public float Value;

        public static PlayerHealthChangeEvent Get(float val)
        {
            var evt = GetPooledInternal();
            evt.Value = val;
            return evt;
        }
    }
}
