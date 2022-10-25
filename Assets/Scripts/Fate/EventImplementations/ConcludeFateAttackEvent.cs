using Events;

namespace Fate.EventImplementations
{
    public class ConcludeFateAttackEvent : Event<ConcludeFateAttackEvent>
    {
        public static ConcludeFateAttackEvent Get()
        {
            return GetPooledInternal();
        }
    }
}