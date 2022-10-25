using Events;
using ResourceManagement.EventImplementations;

namespace ResourceManagement
{
    public static class Resource
    {
        public static void Earn(ResourceType type, int amount)
        {
            using (var evt = EarnResourceEvent.Get(type, amount))
            {
                evt.SendGlobal();
            }
        }

        public static bool TrySpend(ResourceType type, int amount)
        {
            using (var evt = SpendResourceEvent.Get(type, amount))
            {
                evt.SendGlobal();
                return evt.result == EventResult.Positive;
            }
        }
    }
}