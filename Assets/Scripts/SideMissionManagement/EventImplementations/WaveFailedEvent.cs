using Events;

namespace SideMissionManagement.EventImplementations
{
    public class WaveFailedEvent : Event<WaveFailedEvent>
    {
        public int WaveId;
        
        public static WaveFailedEvent Get(int waveId)
        {
            var evt = GetPooledInternal();
            evt.WaveId = waveId;

            return evt;
        }
    }
}