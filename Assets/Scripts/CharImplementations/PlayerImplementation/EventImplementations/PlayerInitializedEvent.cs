using Events;

namespace CharImplementations.PlayerImplementation.EventImplementations
{
    public class PlayerInitializedEvent : Event<PlayerInitializedEvent>
    {
        public static PlayerInitializedEvent Get()
        {
            return GetPooledInternal();
        }
    }
}