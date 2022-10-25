using Events;
using Fate.Modules;

namespace Fate.EventImplementations
{
    public class RemoveModuleFromInventoryEvent : Event<RemoveModuleFromInventoryEvent>
    {
        public ModuleRuntimeData ModuleRuntimeData;

        public static RemoveModuleFromInventoryEvent Get(ModuleRuntimeData moduleRuntimeData)
        {
            var evt = GetPooledInternal();
            evt.ModuleRuntimeData = moduleRuntimeData;

            return evt;
        }
    }
}