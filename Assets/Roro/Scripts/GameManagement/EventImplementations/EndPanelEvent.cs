using Events;
using UnityEngine;

namespace Roro.Scripts.GameManagement.EventImplementations
{
    public class EndPanelEventEvent : Event<EndPanelEventEvent>
    {
        public bool Win;
        public int Score;

        public static EndPanelEventEvent Get(bool result, int score)
        {
            var evt = GetPooledInternal();
            evt.Score = score;
            evt.Win = result;
            return evt;
        }
    }
}