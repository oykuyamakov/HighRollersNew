using Events;

namespace Fate.EventImplementations
{
    /// <summary>
    /// Called on fight start or when F8 modules are set
    /// </summary>
    public class ActivatePassiveEffectsEvent : Event<ActivatePassiveEffectsEvent>
    {
        public static ActivatePassiveEffectsEvent Get()
        {
            return GetPooledInternal();
        }
    }
}