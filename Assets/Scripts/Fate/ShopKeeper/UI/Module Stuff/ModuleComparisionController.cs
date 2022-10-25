using Fate.Modules;
using UnityEngine;

namespace Fate.ShopKeeper.UI
{
    public class ModuleComparisionController : MonoBehaviour
    {
        public ModuleStatsUI NewModuleUI;
        public ModuleStatsUI EquippedModuleUI;

        private const string NewModuleText = "New Module";
        private const string EquippedModuleText = "Equipped Module";

        public void UpdateUI(ModuleRuntimeData selectedModule, ModuleRuntimeData equippedModule)
        {
            NewModuleUI.SetupUI(NewModuleText, selectedModule.GetModuleStatsList(), selectedModule.Data);
            EquippedModuleUI.SetupUI(EquippedModuleText, equippedModule.GetModuleStatsList(), equippedModule.Data);
        }
    }
}