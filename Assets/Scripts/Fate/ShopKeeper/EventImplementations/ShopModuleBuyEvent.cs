using Events;
using Fate.Modules;

namespace Fate.ShopKeeper.EventImplementations
{
    public class ShopModuleBuyEvent : Event<ShopModuleBuyEvent>
    {
        public ModuleShopItem Module;
        
        public static ShopModuleBuyEvent Get(ModuleShopItem module)
        {
            var evt = GetPooledInternal();
            evt.Module = module;

            return evt;
        }
    }
}