using System;
using Fate.Modules;
using Pooling;
using UnityEngine;
using UnityEngine.UI;

namespace Fate.Test
{
    public class FateTester : MonoBehaviour
    {
        public Button AddTestModulesButton;
        public Button AddTestModuleButton;
        public Button ClearInventoryButton;

        public ModuleSelector ModuleSelector;

        private void Awake()
        {
            if (FateSettings.Get().Testing)
            {
                AddTestModulesButton.gameObject.SetActive(true);
                AddTestModuleButton.gameObject.SetActive(true);
            }
            
            AddTestModulesButton.onClick.AddListener(AddTestModules);
            AddTestModuleButton.onClick.AddListener(AddTestModule);
            ClearInventoryButton.onClick.AddListener(OnClearInventory);
        }
        
        public void AddTestModules()
        {
            var tempList = ListPool<ModuleRuntimeData>.Get();

            for (var i = 0; i < ModuleManager.Modules.Count; i++)
            {
                var module = ModuleManager.Modules[i];
                var moduleRuntimeData = new ModuleRuntimeData();
                moduleRuntimeData.Module = module;
                moduleRuntimeData.Tier = ModuleTier.Legendary;
                moduleRuntimeData.Slot = -1;
                tempList.Add(moduleRuntimeData);
            }

            for (var i = 0; i < tempList.Count; i++)
            {
                var module = tempList[i];

                var inventoryItem = new ModuleInventoryItem();
                inventoryItem.ModuleData = module;

                ModuleManager.Instance.ModuleInventorySystem.AddToInventory(inventoryItem);
            }

            ModuleManager.Instance.ModuleInventorySystem.SendModifiedEvent();
        }

        public void AddTestModule()
        {
            ModuleSelector.gameObject.SetActive(true);
        }
        
        private void OnClearInventory()
        {
            ModuleManager.Instance.ModuleInventorySystem.ClearInventory();
        }
        
        //     public Button FateButton;
//     public Button InventoryButton;
//     public Button ShopKeeperButton;
//
//     public CanvasGroup CanvasGroup;
//     
//     private void OnEnable()
//     {
//         FateButton.onClick.AddListener(OpenFate);
// //        InventoryButton.onClick.AddListener(OpenInventory);
//         ShopKeeperButton.onClick.AddListener(OpenShopKeeper);
//         
//         GEM.AddListener<ToggleFateEditorUIEvent>(OnToggleFate);
//         GEM.AddListener<ToggleShopKeeperUIEvent>(OnToggleShop);
//     }
//
//     private void OnToggleFate(ToggleFateEditorUIEvent evt)
//     {
//         if(evt.Visible)
//             return;
//
//         CanvasGroup.Toggle(true, 0.25f);
//     }
//     
//     private void OnToggleShop(ToggleShopKeeperUIEvent evt)
//     {
//         if(evt.Visible)
//             return;
//
//         CanvasGroup.Toggle(true, 0.25f);
//     }
//
//     public void OpenFate()
//     {
//         using var evt = ToggleFateEditorUIEvent.Get(true).SendGlobal();
//
//         CanvasGroup.Toggle(false, 0.25f);
//     }
//
//     public void OpenInventory()
//     {
//         
//     }
//
//     public void OpenShopKeeper()
//     {
//         using var evt = ToggleShopKeeperUIEvent.Get(true).SendGlobal();
//         
//         CanvasGroup.Toggle(false, 0.25f);
//     }
    }
}
