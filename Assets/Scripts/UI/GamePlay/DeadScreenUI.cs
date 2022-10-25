using System;
using System.Collections.Generic;
using CharImplementations.PlayerImplementation.EventImplementations;
using Events;
using GameStages.EventImplementations;
using Roro.Scripts.UI.UITemplates.UITemplateImplementations;
using SceneManagement;
using UnityCommon.Modules;
using UnityCommon.Runtime.UI;
using UnityCommon.Runtime.UI.Animations;
using UnityEngine;
using SceneManagement;


namespace UI.GamePlay
{
    public class DeadScreenUI : MonoBehaviour
    {
        [SerializeField] 
        private Canvas m_Canvas;

        [SerializeField] 
        private List<ScaleAnim> m_ButtonAnims;
        
        [SerializeField]
        private ButtonTemplate m_RestartButtonTemplate;
 
        [SerializeField]
        private ButtonTemplate m_MenuButtonTemplate;

        [SerializeField] 
        private UITranslateAnim m_BackgroundAnim;

        [SerializeField] 
        private UIAlphaAnim m_ComicAlphaAnim;

        void Awake()
        {
            GEM.AddListener<PlayerDeadEvent>(OnPlayerDeadEvent);
            SetButtonFunctions();
        }

        private void OnDestroy()
        {
            GEM.RemoveListener<PlayerDeadEvent>(OnPlayerDeadEvent);
        }

        public void SetButtonFunctions()
        {
            m_RestartButtonTemplate.Set(OnRestartButtonClicked);
            m_MenuButtonTemplate.Set(OnMenuButtonClicked);
        }
        
        public void OnMenuButtonClicked()
        {
            using var evt = SceneChangeRequestEvent.Get(SceneId.Menu);
            evt.SendGlobal();
            FadeInOut.Instance.DoTransition(() => {                
                m_Canvas.enabled = false;

            }, 0.41f, Color.black, 0.1f);
        }
        
        public void OnRestartButtonClicked()
        {
            using var evt = ResetLevelEvent.Get();
            evt.SendGlobal();

            OnBackgroundOut();
                
            FadeInOut.Instance.DoTransition(() => {                
                m_Canvas.enabled = false;

            }, 0.41f, Color.black, 2f);
        }
        
        public void OnBackgroundCome()
        {
            m_ComicAlphaAnim.FadeIn();
            
            for (int i = 0; i < m_ButtonAnims.Count; i++)
            {
                m_ButtonAnims[i].gameObject.SetActive(true);
                m_ButtonAnims[i].FadeOut();
            }
        }
        public void OnBackgroundOut()
        {
            m_ComicAlphaAnim.FadeOut();
            m_BackgroundAnim.FadeOut();
        }
        
        
        private void OnPlayerDeadEvent(PlayerDeadEvent evt)
        {
            m_Canvas.enabled = true;
            m_BackgroundAnim.FadeIn(OnBackgroundCome);
            
            // Conditional.Wait(1f).Do(() =>
            // {
            //     using var evt = ResetLevelEvent.Get();
            //     evt.SendGlobal();
            //     
            //     FadeInOut.Instance.DoTransition(() => {                
            //         m_Canvas.enabled = false;
            //         
            //         
            //     }, 0.41f, Color.black, 1f);
            // });
        }
    }
}
