using Events;
using Fate.Modules;
using UnityEngine;

namespace Fate.EventImplementations
{
    /// <summary>
    /// Attempting to Equip/Unequip
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