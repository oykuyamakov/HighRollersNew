using Events;
using Fate.Modules;

namespace Fate.EventImplementations
{
    public class FateAttackActivatedEvent : Event<FateAttackActivatedEvent>
    {
        public ModuleRuntimeData ActiveModule;

        public static FateAttackActivatedEvent Get(ModuleRuntimeData activeModule)
        {
            var evt = GetPooledInternal();
            evt.ActiveModule = activeModule;

            return evt;
        }
    }
}