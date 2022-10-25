using System;
using DG.Tweening;
using Events;
using Roro.Scripts.UI.UITemplates.UITemplateImplementations;
using SceneManagement;
using UnityCommon.Singletons;
using UnityEngine;

namespace UI.GamePlay.SceneTransition
{
    public class SceneChangeUI : SingletonBehaviour<SceneChangeUI>
    {
        [SerializeField] 
        private Canvas m_Canvas;
        
        [SerializeField] 
        private ButtonTemplate m_YesButton;
        [SerializeField] 
        private ButtonTemplate m_NoButton;

        [SerializeField] 
        private TextMeshProTemplate m_InfoText;

        private SceneId m_SceneToActivate;

        private Action m_OnInput;

        private void Awake()
        {
            if(SetupInstance(false))
                return;
        }

        public void Set(string infoText, SceneId sceneToActive, Action onInput)
        {
            m_SceneToActivate = sceneToActive;
            m_InfoText.Set($"Go to {infoText}");
            m_YesButton.Set(OnYesButton);
            m_NoButton.Set(OnNoButton);

            m_OnInput = onInput;

            Toggle(true);
        }

        private void Toggle(bool enable)
        {
            var scale = enable ? 1 : 0;
            if(enable)
                m_Canvas.enabled = true;
            
            transform.DOScale(Vector3.one * scale,  0.3f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                if(!enable)
                    m_Canvas.enabled = false;
                
                m_OnInput.Invoke();
            });
        }
        
        private void OnYesButton()
        {
            Toggle(false);
            
            using var sceneChangeRequest = SceneChangeRequestEvent.Get(m_SceneToActivate);
            sceneChangeRequest.SendGlobal();
        }

        private void OnNoButton()
        {
            Toggle(false);
        }
    }
}
