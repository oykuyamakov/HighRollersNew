using CharImplementations.PlayerImplementation.EventImplementations;
using Events;
using GameStages.EventImplementations;
using UnityCommon.Modules;
using UnityCommon.Runtime.UI;
using UnityEngine;

namespace UI.GamePlay
{
    public class DeadScreenUI : MonoBehaviour
    {
        [SerializeField] 
        private Canvas m_Canvas;
        
        void Awake()
        {
            GEM.AddListener<PlayerDeadEvent>(OnPlayerDeadEvent);
        }
        
        private void OnPlayerDeadEvent(PlayerDeadEvent evt)
        {
            m_Canvas.enabled = true;
            
            Conditional.Wait(1f).Do(() =>
            {
                using var evt = ResetLevelEvent.Get();
                evt.SendGlobal();
                
                FadeInOut.Instance.DoTransition(() => {                 m_Canvas.enabled = false;
                }, 0.41f, Color.black, 1f);
                
            });
        }
    }
}
