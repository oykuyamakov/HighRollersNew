using Events;
using MoreMountains.NiceVibrations;

namespace Roro.Scripts.GameManagement.EventImplementations
{
    public class AutarkicGiveHapticEvent : Event<AutarkicGiveHapticEvent>
    {
        public float intensity = -1;
        public float sharpness = -1;
        public HapticTypes HapticType = HapticTypes.Selection;
        public static AutarkicGiveHapticEvent Get(float intensity,float sharpness)
        {
            var evt = GetPooledInternal();
            evt.intensity = intensity;
            evt.sharpness = sharpness;
            return evt;
        }
        public static AutarkicGiveHapticEvent Get(HapticTypes hapticType)
        {
            var evt = GetPooledInternal();
            evt.HapticType = hapticType;
            return evt;
        }
    }
}
