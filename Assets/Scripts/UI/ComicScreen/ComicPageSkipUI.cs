using System;
using System.Collections.Generic;
using DG.Tweening;
using Events;
using Roro.Scripts.UI.UITemplates.UITemplateImplementations;
using SceneManagement;
using SettingImplementations;
using UnityCommon.Modules;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ComicScreen
{
    public class ComicPageSkipUI : MonoBehaviour
    {
        [SerializeField] 
        private Image m_ComicPage;
        
        [SerializeField] 
        private Canvas m_Canvas;
        
        [SerializeField] 
        private ButtonTemplate m_NextButton;
        private GeneralSettings m_GeneralSettings => GeneralSettings.Get();

        private SceneId m_SceneToActivate;
        
        private List<Sprite> m_ComicsList;

        private Action m_OnInput;

        private int m_Index;
        private int m_ComicCount;

        private void Awake()
        {
            GEM.AddListener<OnSceneLoadedEvent>(OnSceneLoad);

            Set();
        }

        public void Set()
        {
            m_NextButton.Set(OnNextButton);
        }

        private void Toggle(bool enable)
        {
            var scale = enable ? 1 : 0;
            if (enable)
                m_Canvas.enabled = true;
            
            m_ComicPage.transform.DOScale(Vector3.one * scale,  0.5f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                if(!enable)
                    m_Canvas.enabled = false;
                
                m_OnInput.Invoke();
            });
        }
        
        public void OnNextButton()
        {
            m_Index++;
            
            if (m_Index >= m_ComicCount)
            {
                Toggle(false);

                // Conditional.Wait(1.5f).Do(() =>
                // {
                //     using var chagneScene = SceneChangeRequestEvent.Get(m_SceneToActivate);
                //     chagneScene.SendGlobal();
                // });
                
                return;
            }

            Sequence seq = DOTween.Sequence();
            seq.Append(m_ComicPage.DOFade(0f, 0.2f).OnComplete(() =>
            {
                m_ComicPage.sprite = m_ComicsList[m_Index];
            }));
            seq.Append(m_ComicPage.DOFade(1f, 0.2f));
        }
        private void OnSceneLoad(OnSceneLoadedEvent evt)
        {
            if (m_GeneralSettings.m_ComicsBySceneIds.TryGetValue(evt.SceneController.SceneId, out var list))
            {
                m_SceneToActivate = evt.SceneController.SceneId;
                
                Conditional.Wait(1.5f).Do(() =>
                {
                    m_Index = 0;
                    m_ComicsList = list;
                    m_ComicCount = m_ComicsList.Count;
                    m_ComicPage.sprite = m_ComicsList[m_Index];

                    Toggle(true);
                    
                    // using var chagneScene = SceneChangeRequestEvent.Get(SceneId.Comic);
                    // chagneScene.SendGlobal();
                });
            }
        }
    }
}
