using System;
using System.Collections.Generic;
using Fate.Modules;
using Fate.PassiveSkills;
using UnityCommon.Runtime.Extensions;
using UnityEngine;

namespace Fate.ShopKeeper.UI.Stats
{
    [Serializable]
    public struct ModuleStatPair
    {
        public Module Module;
        public float Value;

        public ModuleStatPair(Module module, float val)
        {
            Module = module;
            Value = val;
        }
    }

    public class ShopStatsBarUI : MonoBehaviour
    {
        Dictionary<StatType, List<ModuleStatPair>> Stats = new Dictionary<StatType, List<ModuleStatPair>>();

        public List<ShopStatsBarItemUI> StatsBarItemUis = new List<ShopStatsBarItemUI>();

        [SerializeField]
        private List<GameObject> m_StatBarItemPrefabs = new List<GameObject>();

        private void OnEnable()
        {
            SetupStatValues();
            SetupBar();
        }

        private void SetupStatValues()
        {
            Stats = FateManager.GetEquippedModuleStatPairs();
        }

        // TODO: not instantiate
        private void SetupBar()
        {
            foreach (var stat in Stats)
            {
                var statLevelObj = Instantiate(m_StatBarItemPrefabs[stat.Value.Count - 1], transform);

                var statBarItemUi = statLevelObj.GetComponent<ShopStatsBarItemUI>();
                statBarItemUi.Setup(stat.Value);

                StatsBarItemUis.Add(statBarItemUi);
            }
        }

        private void OnDisable()
        {
            var count = StatsBarItemUis.Count - 1;
            for (var i = count; i >= 0; i--)
            {
                StatsBarItemUis[i].Destroy();
            }

            StatsBarItemUis.Clear();
        }
    }
}