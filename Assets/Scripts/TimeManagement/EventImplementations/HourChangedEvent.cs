using Events;

namespace TimeManagement.EventImplementations
{
    public class HourChangedEvent : Event<HourChangedEvent>
    {
        public static HourChangedEvent Get()
        {
            var evt = GetPooledInternal();

            return evt;
        }
    }
}