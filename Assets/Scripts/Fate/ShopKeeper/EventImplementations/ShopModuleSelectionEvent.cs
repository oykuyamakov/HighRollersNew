using Events;
using Fate.Modules;

namespace Fate.ShopKeeper.EventImplementations
{
    public class ShopModuleSelectionEvent : Event<ShopModuleSelectionEvent>
    {
        public ModuleShopItem SelectedModule;

        public static ShopModuleSelectionEvent Get(ModuleShopItem selectedModule)
        {
            var evt = GetPooledInternal();
            evt.SelectedModule = selectedModule;

            return evt;
        }
    }
}