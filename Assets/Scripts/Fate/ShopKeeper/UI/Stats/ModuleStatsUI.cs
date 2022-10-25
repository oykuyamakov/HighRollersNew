using System.Collections.Generic;
using Fate.Modules.Data;
using TMPro;
using UnityEngine;

namespace Fate.ShopKeeper.UI
{
    public class ModuleStatsUI : MonoBehaviour
    {
        public TextMeshProUGUI InfoText;
        public StatsGroupUI StatsGroupUI;
        public ShopModuleInfoUI ModuleInfoUI;

        public void SetupUI(string infoText, List<int> stats, ModuleData moduleData)
        {
            InfoText.text = infoText;
            StatsGroupUI.SetStats(stats);
            ModuleInfoUI.SetModuleInfo(moduleData);
        }
    }
}