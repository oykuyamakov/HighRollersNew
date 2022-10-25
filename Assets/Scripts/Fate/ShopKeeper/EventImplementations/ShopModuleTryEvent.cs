using Events;
using Fate.Modules;

namespace Fate.ShopKeeper.EventImplementations
{
    public class ShopModuleTryEvent : Event<ShopModuleTryEvent>
    {
        public ModuleShopItem Module;
        
        public static ShopModuleTryEvent Get(ModuleShopItem module)
        {
            var evt = GetPooledInternal();
            evt.Module = module;

            return evt;
        }
    }
}