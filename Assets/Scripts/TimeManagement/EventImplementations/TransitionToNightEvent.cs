using Events;

namespace TimeManagement.EventImplementations
{
    public class TransitionToNightEvent : Event<TransitionToNightEvent>
    {
        public static TransitionToNightEvent Get()
        {
            var evt = GetPooledInternal();

            return evt;
        }
    }
}