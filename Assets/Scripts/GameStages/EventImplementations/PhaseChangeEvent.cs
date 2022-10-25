using Events;
using UnityEngine;

namespace GameStages.EventImplementations
{
    public class PhaseChangeEvent : Event<PhaseChangeEvent>
    {
        public int PhaseIndex;

        public static PhaseChangeEvent Get(int phaseIndex)
        {
            var evt = GetPooledInternal();
            evt.PhaseIndex = phaseIndex;
            return evt;
        }
    }
}
