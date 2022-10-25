using Events;

namespace Fate.EventImplementations
{
    /// <summary>
    /// Called when fate attack is called
    /// </summary>
    public class CallFateAttackEvent : Event<CallFateAttackEvent>
    {
        public static CallFateAttackEvent Get()
        {
            return GetPooledInternal();
        }
    }
}