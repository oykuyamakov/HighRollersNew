using CharImplementations.PlayerImplementation;
using CharImplementations.PlayerImplementation.EventImplementations;
using Events;

namespace CharImplementations
{
    public static class PlayerExtensions
    {
        public static Player GetPlayer()
        {
            using var evt = GetPlayerEvent.Get().SendGlobal();

            return evt.Player;
        }
    }
}