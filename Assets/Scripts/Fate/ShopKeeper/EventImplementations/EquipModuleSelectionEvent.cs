using Events;
using Fate.ShopKeeper.UI.Module_Stuff;

namespace Fate.ShopKeeper.EventImplementations
{
    public class EquipModuleSelectionEvent : Event<EquipModuleSelectionEvent>
    {
        public ModuleEquipUI ModuleEquipUI;

        public static EquipModuleSelectionEvent Get(ModuleEquipUI moduleEquipUI)
        {
            var evt = GetPooledInternal();
            evt.ModuleEquipUI = moduleEquipUI;

            return evt;
        }
    }
}