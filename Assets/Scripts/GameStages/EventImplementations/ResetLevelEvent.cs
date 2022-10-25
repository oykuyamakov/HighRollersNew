using Events;
using UnityEngine;

namespace GameStages.EventImplementations
{
    public class ResetLevelEvent : Event<ResetLevelEvent>
    {
        public static ResetLevelEvent Get()
        {
            var evt = GetPooledInternal();
            return evt;
        }
    }
}
