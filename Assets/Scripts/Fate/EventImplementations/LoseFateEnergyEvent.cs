using Events;

namespace Fate.EventImplementations
{
    public class LoseFateEnergyEvent : Event<LoseFateEnergyEvent>
    {
        public int Amount;

        public static LoseFateEnergyEvent Get(int amount)
        {
            var evt = GetPooledInternal();
            evt.Amount = amount;
            return evt;
        }
    }
}