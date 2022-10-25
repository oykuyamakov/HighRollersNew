using Events;
using Fate.Modules;

namespace Fate.EventImplementations
{
    /// <summary>
    /// Attempting to Equip/Unequip
    /// (equip if unequipped)
    /// </summary>
    public class ModuleEquipmentEvent : Event<ModuleEquipmentEvent>
    {
        public ModuleRuntimeData SelectedModule;

        public static ModuleEquipmentEvent Get(ModuleRuntimeData selectedModule)
        {
            var evt = GetPooledInternal();
            evt.SelectedModule = selectedModule;
            return evt;
        }
    }
}