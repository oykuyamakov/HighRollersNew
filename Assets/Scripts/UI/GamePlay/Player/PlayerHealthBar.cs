using System.Globalization;
using CharImplementations.PlayerImplementation;
using CharImplementations.PlayerImplementation.EventImplementations;
using Events;
using Roro.Scripts.UI.UITemplates.UITemplateImplementations;
using Unity.Mathematics;
using UnityEngine;

namespace UI.GamePlay.Player
{
    public class PlayerHealthBar : MonoBehaviour
    {
        [SerializeField]
        public TextMeshProTemplate m_HealthText;

        [SerializeField] 
        private SliderTemplate m_SliderTemplate;

        private void Awake()
        {
            GEM.AddListener<PlayerHealthChangeEvent>(OnHealthChangeEvent);
        }

        private void OnDestroy()
        {
            GEM.RemoveListener<PlayerHealthChangeEvent>(OnHealthChangeEvent);
        }

        private void OnHealthChangeEvent(PlayerHealthChangeEvent evt)
        {
            SetHealthValues(evt.Value);
        }

        public void SetHealthValues(float health)
        {
            var h = (int)health;
            m_HealthText.Set(h.ToString(CultureInfo.InvariantCulture));
            m_SliderTemplate.AnimatedSet((int)health, 0.1f, CharImplementations.PlayerImplementation.Player.PlayerHealth);
        }
    }
}
