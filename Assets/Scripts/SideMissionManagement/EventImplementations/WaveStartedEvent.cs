using Events;

namespace SideMissionManagement.EventImplementations
{
    public class WaveStartedEvent : Event<WaveStartedEvent>
    {
        public int WaveId;
        
        public static WaveStartedEvent Get(int waveId)
        {
            var evt = GetPooledInternal();
            evt.WaveId = waveId;

            return evt;
        }
    }
}