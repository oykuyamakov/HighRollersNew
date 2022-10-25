using Events;
using Fate.Modules;
using Fate.ShopKeeper.EventImplementations;
using UnityEngine;
using UnityEngine.UI;

namespace Fate.ShopKeeper.UI.Module_Stuff
{
    public class EquipModuleController : MonoBehaviour
    {
        public ModuleComparisionController ModuleComparisionController;
        
        public ModuleRuntimeData NewModule;
        public ModuleEquipUI Selected;

        public Button ConfirmButton;
        public Button CancelButton;

        private void OnEnable()
        {
            ConfirmButton.onClick.AddListener(OnConfirm);
            CancelButton.onClick.AddListener(OnCancel);
            
            GEM.AddListener<EquipModuleSelectionEvent>(OnModuleSelected);
        }

        private void OnModuleSelected(EquipModuleSelectionEvent evt)
        {
            if (Selected != null)
            {
                Selected.OnDeselected();
            }

            Selected = evt.ModuleEquipUI;
            ModuleComparisionController.UpdateUI(NewModule, Selected.ModuleData);
        }

        private void OnConfirm()
        {
            Selected.ModuleData.EquipModule();
            NewModule.EquipModule();
        }

        private void OnCancel()
        {
            
        }

        private void OnDisable()
        {
            ConfirmButton.onClick.RemoveListener(OnConfirm);
            CancelButton.onClick.RemoveListener(OnCancel);
            
            GEM.RemoveListener<EquipModuleSelectionEvent>(OnModuleSelected);
        }
    }
}