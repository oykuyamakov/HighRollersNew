using Events;
using Fate.ShopKeeper.UI;

namespace Fate.ShopKeeper.EventImplementations
{
    public class ShopSidebarTabSelectedEvent : Event<ShopSidebarTabSelectedEvent>
    {
        public ShopTabUI ShopTabUI;
        
        public static ShopSidebarTabSelectedEvent Get(ShopTabUI tabUI)
        {
            var evt = GetPooledInternal();
            evt.ShopTabUI = tabUI;

            return evt;
        }
    }
}