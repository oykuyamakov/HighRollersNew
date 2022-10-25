using Events;

namespace Fate.EventImplementations
{
    public class CancelFateAttackEvent : Event<CancelFateAttackEvent>
    {
        public CancelFateAttackEvent Get()
        {
            var evt = GetPooledInternal();
            return evt;
        }
    }
}