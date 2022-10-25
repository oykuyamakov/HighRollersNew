using System.Collections.Generic;
using Events;
using Fate.Modules;
using Fate.PassiveSkills;
using Fate.ShopKeeper.EventImplementations;
using Pooling;
using UnityEngine;

namespace Fate.ShopKeeper.UI
{
    public class StatsGroupUI : MonoBehaviour
    {
        public List<StatUI> StatUis = new List<StatUI>();
        
        public Dictionary<StatType, StatUI> StatUiDictionary = new Dictionary<StatType, StatUI>();

        private void OnEnable()
        {
            GEM.AddListener<ShopModuleSelectionEvent>(OnModuleSelection);
            
            SetStats();
        }

        public void SetStats()
        {
            var stats = FateManager.GetEquippedModuleStats();
            
            for (var i = 0; i < StatUis.Count; i++)
            {
                StatUis[i].SetStat((StatType)i, stats[i]);
                StatUiDictionary.Add((StatType)i, StatUis[i]);
            }
            
            ListPool<int>.Release(stats);
        }

        public void SetStats(List<int> stats)
        {
            for (var i = 0; i < StatUis.Count; i++)
            {
                StatUis[i].SetStat((StatType)i, stats[i]);
                StatUiDictionary.Add((StatType)i, StatUis[i]);
            }
        }

        private void OnModuleSelection(ShopModuleSelectionEvent evt)
        {
            UpdateStats(evt.SelectedModule.ModuleData);
        }
        
        public void UpdateStats(ModuleRuntimeData moduleRuntimeData)
        {
            for (var i = 0; i < StatUis.Count; i++)
            {
                StatUis[i].UpdateData((int) moduleRuntimeData.GetPassiveStatValue((StatType) i));
            }
        }
    }
}