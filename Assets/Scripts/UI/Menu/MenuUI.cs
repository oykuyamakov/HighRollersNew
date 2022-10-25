using System;
using Events;
using SceneManagement;
using SettingImplementations;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class MenuUI : MonoBehaviour
    {
        [SerializeField] 
        private Button m_PlayDemoButton;

        private void Start()
        {
            SetButtons();

            Debug.Log("why");
        }

        public void SetButtons()
        {
            m_PlayDemoButton.onClick.RemoveAllListeners();
            m_PlayDemoButton.onClick.AddListener(OnPlayDemo);
            m_PlayDemoButton.enabled = true;
        }

        public void OnPlayDemo()
        {
            m_PlayDemoButton.enabled = false;
            SceneTransitionManager.Instance.ChangeScene(SceneId.Hub);
        }
    }
}
