using Events;
using Fate.Modules.Data;
using Fate.ShopKeeper.EventImplementations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fate.ShopKeeper.UI
{
    public class ShopModuleInfoUI : MonoBehaviour
    {
        public Image ModuleIcon;
        public TextMeshProUGUI ModuleDescriptionText;

        private void OnEnable()
        {
            GEM.AddListener<ShopModuleSelectionEvent>(SetModuleInfo);
        }

        public void SetModuleInfo(ModuleData moduleData)
        {
            ModuleIcon.sprite = moduleData.ModuleIcon;
            ModuleDescriptionText.text = moduleData.ModuleDescription;
        }

        public void SetModuleInfo(ShopModuleSelectionEvent evt)
        {
            ModuleIcon.sprite = evt.SelectedModule.ModuleData.Data.ModuleIcon;
            ModuleDescriptionText.text = evt.SelectedModule.ModuleData.Data.ModuleDescription;
        }
        
        private void OnDisable()
        {
            GEM.RemoveListener<ShopModuleSelectionEvent>(SetModuleInfo);
        }
    }
}