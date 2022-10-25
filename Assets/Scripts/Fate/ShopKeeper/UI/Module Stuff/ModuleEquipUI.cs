using Events;
using Fate.Modules;
using Fate.ShopKeeper.EventImplementations;
using UnityEngine;
using UnityEngine.UI;

namespace Fate.ShopKeeper.UI.Module_Stuff
{
    public class ModuleEquipUI : MonoBehaviour
    {
        public ModuleRuntimeData ModuleData;
        
        public Image ModuleIcon;
        public Image SelectedOutline;
        public Button SelectButton;

        private bool m_Selected;
        
        private void OnEnable()
        {
            SelectButton.onClick.AddListener(OnSelected);
        }

        public void UpdateUI(ModuleRuntimeData data)
        {
            ModuleData = data;
            ModuleIcon.sprite = data.Data.ModuleIcon;
        }

        public void OnSelected()
        {
            m_Selected = true;
            SelectedOutline.enabled = true;

            using var evt = EquipModuleSelectionEvent.Get(this).SendGlobal();
        }

        public void OnDeselected()
        {
            m_Selected = false;
            SelectedOutline.enabled = false;
        }
        
        private void OnDisable()
        {
            SelectButton.onClick.RemoveListener(OnSelected);
        }
    }
}