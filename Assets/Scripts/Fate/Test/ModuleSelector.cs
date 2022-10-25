using Events;
using Fate.EventImplementations;
using Fate.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fate.Test
{
    public class ModuleSelector : MonoBehaviour
    {
        public TMP_Dropdown ModuleDropdown;
        public TMP_Dropdown ModuleRarityDropdown;

        public Button AddButton;

        private void OnEnable()
        {
            for (var i = 0; i < ModuleManager.Modules.Count; i++)
            {
                var module = ModuleManager.Modules[i].Data;

                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(module.ModuleName, module.ModuleIcon);
                ModuleDropdown.options.Add(optionData);
            }

            ModuleRarityDropdown.options.Add(new TMP_Dropdown.OptionData(ModuleTier.Common.ToString()));
            ModuleRarityDropdown.options.Add(new TMP_Dropdown.OptionData(ModuleTier.Uncommon.ToString()));
            ModuleRarityDropdown.options.Add(new TMP_Dropdown.OptionData(ModuleTier.Rare.ToString()));
            ModuleRarityDropdown.options.Add(new TMP_Dropdown.OptionData(ModuleTier.Epic.ToString()));
            ModuleRarityDropdown.options.Add(new TMP_Dropdown.OptionData(ModuleTier.Legendary.ToString()));

            AddButton.onClick.AddListener(OnAdd);
        }

        public void OnAdd()
        {
            // TODO: i want this
            // var newModuleRuntimeData = ModuleRuntimeData.Create(ModuleManager.Modules[ModuleDropdown.value].Data, (ModuleTier) ModuleRarityDropdown.value);

            var newModuleRuntimeData = new ModuleRuntimeData();
            newModuleRuntimeData.Module = ModuleManager.Modules[ModuleDropdown.value];
            newModuleRuntimeData.Tier = (ModuleTier)ModuleRarityDropdown.value;

            using var evt = AddModuleToInventoryEvent.Get(newModuleRuntimeData).SendGlobal();

            gameObject.SetActive(false);
        }
    }
}