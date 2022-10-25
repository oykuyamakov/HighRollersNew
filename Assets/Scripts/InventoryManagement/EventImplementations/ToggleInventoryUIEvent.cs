using Events;

namespace InventoryManagement.EventImplementations
{
    public class ToggleInventoryUIEvent : Event<ToggleInventoryUIEvent>
    {
        public bool Visible;

        public static ToggleInventoryUIEvent Get(bool visible)
        {
            var evt = GetPooledInternal();
            evt.Visible = visible;
            return evt;
        }
    }
}