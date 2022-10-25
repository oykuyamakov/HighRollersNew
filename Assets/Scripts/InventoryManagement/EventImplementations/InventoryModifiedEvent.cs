using Events;

namespace InventoryManagement.EventImplementations
{
    public class InventoryModifiedEvent : Event<InventoryModifiedEvent>
    {
        public static InventoryModifiedEvent Get()
        {
            return GetPooledInternal();
        }
    }
}