using System;
using Events;
using ResourceManagement.EventImplementations;
using Roro.Scripts.Serialization;
using UnityCommon.Variables;
using UnityEngine;

namespace ResourceManagement
{
    public class ResourceManager : MonoBehaviour
    {
        [SerializeField]
        private IntVariable m_Dice;
        
        [SerializeField]
        private IntVariable m_Chip;

        private SerializationWizard m_SerializationContext;

        private void OnEnable()
        {
            GEM.AddListener<EarnResourceEvent>(OnEarnResource, Priority.Normal);
            GEM.AddListener<SpendResourceEvent>(OnSpendResource, Priority.Normal);
        }
        
        public IntVariable GetResourceVariable(ResourceType type)
        {
	        return type switch
	        {
		        ResourceType.Dice => m_Dice,
		        ResourceType.Chip => m_Chip,
		        _ => throw new Exception($"Cannot find variable for resource type {type}")
	        };
        }
        
        private void OnEarnResource(EarnResourceEvent evt)
        {
	        var variable = GetResourceVariable(evt.Type);
	        variable.Value += evt.Amount;
	        Variable.SavePlayerPrefs();
        }
        
        private void OnSpendResource(SpendResourceEvent evt)
        {
	        var variable = GetResourceVariable(evt.Type);

	        if (variable.Value < evt.Amount)
	        {
		        evt.result = EventResult.Negative;
		        return;
	        }

	        evt.result = EventResult.Positive;

	        variable.Value -= evt.Amount;
        }
        
        private void OnDisable()
        {
            GEM.RemoveListener<EarnResourceEvent>(OnEarnResource);
            GEM.RemoveListener<SpendResourceEvent>(OnSpendResource);
        }
    }
}
