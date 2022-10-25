using Events;

namespace TimeManagement.EventImplementations
{
    public class MinuteChangedEvent : Event<MinuteChangedEvent>
    {
        public static MinuteChangedEvent Get()
        {
            var evt = GetPooledInternal();

            return evt;
        }
    }
}