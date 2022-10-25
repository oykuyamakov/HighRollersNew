using Events;

namespace Fate.EventImplementations
{
    // TODO: i don't like the naming
    public class ToggleFateEditorUIEvent : Event<ToggleFateEditorUIEvent>
    {
        public bool Visible;

        public static ToggleFateEditorUIEvent Get(bool visible)
        {
            var evt = GetPooledInternal();
            evt.Visible = visible;
            return evt;
        }
    }
}