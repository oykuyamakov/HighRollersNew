using Events;
using Fate.Modules;

namespace Fate.ShopKeeper.EventImplementations
{
    public class ShopModuleEquipEvent : Event<ShopModuleEquipEvent>
    {
        public ModuleShopItem Module;
        
        public static ShopModuleEquipEvent Get(ModuleShopItem module)
        {
            var evt = GetPooledInternal();
            evt.Module = module;

            return evt;
        }
    }
}