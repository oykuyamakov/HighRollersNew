using Events;

namespace TimeManagement.EventImplementations
{
    public class TransitionToDayEvent : Event<TransitionToDayEvent>
    {
        public static TransitionToDayEvent Get()
        {
            var evt = GetPooledInternal();

            return evt;
        }
    }
}