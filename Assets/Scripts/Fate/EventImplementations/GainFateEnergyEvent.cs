using Events;

namespace Fate.EventImplementations
{
    public class GainFateEnergyEvent : Event<GainFateEnergyEvent>
    {
        public int Amount;

        public static GainFateEnergyEvent Get(int amount)
        {
            var evt = GetPooledInternal();
            evt.Amount = amount;
            return evt;
        }
    }
}