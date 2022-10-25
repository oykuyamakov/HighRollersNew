using Events;

namespace Fate.EventImplementations
{
    public class EquippedModulesModifiedEvent : Event<EquippedModulesModifiedEvent>
    {
        public static EquippedModulesModifiedEvent Get()
        {
            return GetPooledInternal();
        }
    }
}