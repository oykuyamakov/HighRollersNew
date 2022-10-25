using Events;

namespace Fate.ShopKeeper.EventImplementations
{
    public class UpdateShopItemUIEvent : Event<UpdateShopItemUIEvent>
    {
        public static UpdateShopItemUIEvent Get()
        {
            return GetPooledInternal();
        }
    }
}