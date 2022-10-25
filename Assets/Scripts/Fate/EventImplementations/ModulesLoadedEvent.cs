using Events;

namespace Fate.EventImplementations
{
    public class ModulesLoadedEvent : Event<ModulesLoadedEvent>
    {
        public static ModulesLoadedEvent Get()
        {
            return GetPooledInternal();
        }
    }
}