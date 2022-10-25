using Events;

namespace CharImplementations.PlayerImplementation.EventImplementations
{
    public class GetPlayerEvent : Event<GetPlayerEvent>
    {
        public Player Player;
        
        public static GetPlayerEvent Get()
        {
            return GetPooledInternal();
        }
    }
}