using System;
using CharImplementations.PlayerImplementation.EventImplementations;
using Events;
using Roro.Scripts.UI.UITemplates;
using Roro.Scripts.UI.UITemplates.UITemplateImplementations;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GamePlay.Player
{
    public class WeaponIndicatorsUI : MonoBehaviour
    {
        [SerializeField] 
        private ImageTemplate m_IndicatorImage1;

        [SerializeField] 
        private TextMeshProTemplate m_NameText1;
        
        [SerializeField] 
        private TextMeshProTemplate m_MagazineText1;
        
        [SerializeField] 
        private ImageTemplate m_IndicatorImage2;
        
        [SerializeField] 
        private TextMeshProTemplate m_NameText2;
        
        [SerializeField] 
        private TextMeshProTemplate m_MagazineText2;

        
        private void Awake()
        {
            GEM.AddListener<WeaponChangeEvent>(OnWeaponChange);
        }

        private void OnDestroy()
        {
            GEM.RemoveListener<WeaponChangeEvent>(OnWeaponChange);
        }

        public void OnWeaponChange(WeaponChangeEvent evt)
        {
            Set(evt.WeaponData.Preview,evt.WeaponData.WeaponName);
        }

        public void Set(Sprite prev, string name)
        {
            m_NameText1.Set(name);
            m_IndicatorImage1.Set(prev);
        }
    }
}
