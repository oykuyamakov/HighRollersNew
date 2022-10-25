using System;
using DG.Tweening;
using Events;
using Roro.Scripts.UI.UITemplates.UITemplateImplementations;
using SceneManagement;
using UnityCommon.Modules;
using UnityCommon.Singletons;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Menu
{
    public class GameMenuUI : SingletonBehaviour<GameMenuUI>
    {
        [SerializeField] private ButtonTemplate m_MainMenuButton;
        [SerializeField] private ButtonTemplate m_HubButton;

        [SerializeField] private Canvas m_CanvasMenu;

        [SerializeField] private Transform m_Panel;

        [SerializeField] private ButtonTemplate m_EnableMenu;

        [SerializeField] private Canvas m_ParentCanvas;

        private bool m_Enabled;

        private Tweener m_PanelTween;

        private void Awake()
        {
            SetButtons();
        }

        private void SetButtons()
        {
            m_MainMenuButton.Set(GoToMenu);
            m_HubButton.Set(GoToHub);
            m_EnableMenu.Set(Toggle);
        }

        public void ToggleCompletely(bool enable)
        {
            m_ParentCanvas.enabled = enable;
        }

        private void Toggle()
        {
            Toggle(!m_Enabled);
        }

        private void GoToHub()
        {
            using var evt = SceneChangeRequestEvent.Get(SceneId.Hub);
            evt.SendGlobal();
        }   
        private void GoToMenu()
        {
            using var evt = SceneChangeRequestEvent.Get(SceneId.Menu);
            evt.SendGlobal();
        }
        public void Toggle(bool enable)
        {
            //Debug.Log("why");

            var scale = enable ? 1 : 0;

            var currScene = SceneTransitionManager.Instance.CurrentSceneId;
            
            m_Enabled = enable;
            //m_PanelTween?.Kill();
            if (enable)
            {
                m_CanvasMenu.enabled = true;

                if (currScene == SceneId.Hub)
                {
                    m_HubButton.enabled = false;
                    m_MainMenuButton.enabled = true;
                }

                if (currScene == SceneId.BossOne)
                {
                    m_HubButton.enabled = false;
                    m_MainMenuButton.enabled = true;
                }
            }
            
            m_Panel.gameObject.SetActive(enable);
            
            m_CanvasMenu.enabled = enable;

            
            // m_PanelTween = m_Panel.DOScale(Vector3.one * scale,  0.2f).SetEase(Ease.InQuad).OnComplete(() =>
            // {
            //     if(!enable)
            //         m_CanvasMenu.enabled = false;
            // });
        }

    }
}
