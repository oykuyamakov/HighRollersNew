using Events;

namespace InventoryManagement.EventImplementations
{
    // TODO: not called
    public class InventoryItemModifiedEvent : Event<InventoryItemModifiedEvent>
    {
        public static InventoryItemModifiedEvent Get()
        {
            return GetPooledInternal();
        }
    }
}