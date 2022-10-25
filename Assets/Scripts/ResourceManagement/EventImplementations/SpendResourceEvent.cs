using Events;

namespace ResourceManagement.EventImplementations
{
    public class SpendResourceEvent : Event<SpendResourceEvent>
    {
        public ResourceType Type;
        public int Amount;

        public static SpendResourceEvent Get(ResourceType type, int amount)
        {
            var evt = GetPooledInternal();
            evt.Type = type;
            evt.Amount = amount;

            return evt;
        }
    }
}