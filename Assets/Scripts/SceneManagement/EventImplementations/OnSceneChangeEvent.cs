using Events;
using UnityEngine;

namespace SceneManagement.EventImplementations
{
    public class OnSceneChangeEvent : Event<OnSceneChangeEvent>
    {
        public SceneId newScene;
        public SceneId transitionScene;
        public bool animate = true;
        public static OnSceneChangeEvent Get(SceneId newScene, SceneId transitionScene)
        {
            var evt = GetPooledInternal();
            evt.newScene = newScene;
            evt.transitionScene = transitionScene;
            return evt;
        }
    }
}
