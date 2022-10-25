using Events;
using Fate.Modules;

namespace Fate.EventImplementations
{
    /// <summary>
    /// Called when Fate energy is full
    /// </summary>
    public class FateEnergyFullEvent : Event<FateEnergyFullEvent>
    {
        public ModuleRuntimeData RolledModule;
        
        public static FateEnergyFullEvent Get()
        {
            return GetPooledInternal();
        }
    }
}