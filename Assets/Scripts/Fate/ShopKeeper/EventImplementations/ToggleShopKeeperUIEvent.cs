using Events;

namespace Fate.ShopKeeper.EventImplementations
{
    // TODO: i don't like the naming
    public class ToggleShopKeeperUIEvent : Event<ToggleShopKeeperUIEvent>
    {
        public bool Visible;

        public static ToggleShopKeeperUIEvent Get(bool visible)
        {
            var evt = GetPooledInternal();
            evt.Visible = visible;
            return evt;
        }
    }
}