using System;
using CharImplementations.PlayerImplementation.EventImplementations;
using CombatManagement.WeaponImplementation;
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
        private TextMeshProTemplate m_RemainingMagazineText1;
        
        [SerializeField] 
        private ImageTemplate m_IndicatorImage2;
        
        [SerializeField] 
        private TextMeshProTemplate m_NameText2;
        
        [SerializeField] 
        private TextMeshProTemplate m_MagazineText2;
        
        [SerializeField] 
        private TextMeshProTemplate m_RemainingMagazineText2;

        
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
            SetInitial(evt.WeaponData);
            SetSecondary(evt.PreviousWeaponData);
        }

        public void SetInitial(WeaponData data)
        {
            m_NameText1.Set(data.WeaponName);
            m_IndicatorImage1.Set(data.Preview);
            m_MagazineText1.Set(data.GetMagazineSize().ToString());
            m_RemainingMagazineText1.Set(data.RemainingMagazine.ToString());
        }
        
        public void SetSecondary(WeaponData data)
        {
            m_NameText2.Set(data.WeaponName);
            m_IndicatorImage2.Set(data.Preview);
            m_MagazineText2.Set(data.GetMagazineSize().ToString());
            m_RemainingMagazineText2.Set(data.RemainingMagazine.ToString());
        }
        
        
        
    }
}
