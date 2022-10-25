using System;
using System.Collections;
using Events;
using Roro.Scripts.GameManagement.EventImplementations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Roro.Scripts.Helpers
{
    public class SpriteSheetAnimator : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] m_AptalSprites;

        [SerializeField]
        private bool m_Loop;

        [SerializeField]
        [ShowIf("@m_Loop")]
        private float m_Duration = 1f;

        [SerializeField]
        [ShowIf("@!m_Loop")]
        private float m_Gap = 0.01f;

        private Image image => GetComponent<Image>();
        private int index = 0;
        private float timer = 0;

        private void Awake()
        {
            GEM.AddListener<EndPanelEventEvent>(OnLevelEnd);
        }

        public void EnableSelf()
        {
            image.enabled = true;
        }
        public void DisableSelf()
        {
            image.enabled = false;
            index = 0;
        }

        public IEnumerator AnimateSplash(Action oncomplete)
        {
            image.enabled = true;
            for (int i = 0; i < m_AptalSprites.Length; i++)
            {
                image.sprite = m_AptalSprites[i];
                yield return new WaitForSeconds(m_Gap);
            }
            
            image.enabled = false;
            oncomplete.Invoke();
        }

        private void OnLevelEnd(EndPanelEventEvent evt)
        {
            DisableSelf();
        }

        [Button]
        public void LoopAnim()
        {
            if((timer+=Time.deltaTime) >= (m_Duration / m_AptalSprites.Length))
            {
                timer = 0;
                image.sprite = m_AptalSprites[index];
                index = (index + 1) % m_AptalSprites.Length;
            }
        }
    }
}
