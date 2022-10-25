using Events;

namespace ResourceManagement.EventImplementations
{
    public class EarnResourceEvent : Event<EarnResourceEvent>
    {
        public ResourceType Type;
        public int Amount;

        public static EarnResourceEvent Get(ResourceType type, int amount)
        {
            var evt = GetPooledInternal();
            evt.Type = type;
            evt.Amount = amount;

            return evt;
        }
    }
}