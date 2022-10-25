using Events;
using Fate.Modules;

namespace Fate.EventImplementations
{
    public class AddModuleToInventoryEvent : Event<AddModuleToInventoryEvent>
    {
        public ModuleRuntimeData ModuleRuntimeData;

        public static AddModuleToInventoryEvent Get(ModuleRuntimeData moduleRuntimeData)
        {
            var evt = GetPooledInternal();
            evt.ModuleRuntimeData = moduleRuntimeData;

            return evt;
        }
    }
}