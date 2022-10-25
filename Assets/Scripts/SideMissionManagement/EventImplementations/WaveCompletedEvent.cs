using Events;

namespace SideMissionManagement.EventImplementations
{
    public class WaveCompletedEvent : Event<WaveCompletedEvent>
    {
        public int WaveId;
        
        public static WaveCompletedEvent Get(int waveId)
        {
            var evt = GetPooledInternal();
            evt.WaveId = waveId;

            return evt;
        }
    }
}