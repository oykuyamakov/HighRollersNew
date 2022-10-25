using System.Collections.Generic;
using Events;
using Fate.EventImplementations;
using Fate.Modules;
using Fate.ShopKeeper.EventImplementations;
using Rewired.Integration.UnityUI;
using UnityCommon.Modules;
using UnityEngine;
using UnityEngine.UI;
using Utility.Extensions;

namespace Fate.ShopKeeper
{
    public class ShopKeeperCanvasController : MonoBehaviour
    {
        public static Dictionary<ModuleCategory, List<Module>> ModulesByCategory =
            new Dictionary<ModuleCategory, List<Module>>();
        
        public ModuleShopController ShopController;

        public CanvasGroup SelfCanvasGroup;
        
        [Header("Canvas Groups")]
        public CanvasGroup SidebarCanvasGroup;
        
        [Header("Canvases")]
        public Canvas SidebarSelectModuleGroup;
        public Canvas SidebarEquipModuleGroup;
        public Canvas MainSelectModuleGroup;
        public Canvas MainEquipModuleGroup;

        [Header("Buttons")]
        public Button ConfirmButton;
        public Button CancelButton;
        public Button StatsButton;
        public Button VideoButton;

        private void OnEnable()
        {
            GetModules();
            GEM.AddListener<ToggleShopKeeperUIEvent>(OnToggleShopKeeperUI);
            GEM.AddListener<ShopModuleEquipEvent>(OnEquipModuleEvent);
        }

        private void GetModules()
        {
            if (!ModuleManager.ModulesLoaded)
            {
                GEM.AddListener<ModulesLoadedEvent>(OnModulesLoaded);
            }
            else
            {
                OnModulesLoaded(null);
            }
        }

        private void OnModulesLoaded(ModulesLoadedEvent evt)
        {
            ShopController.FillInventory(ModuleManager.Modules);
        }

        private void OnToggleShopKeeperUI(ToggleShopKeeperUIEvent evt)
        {
            SelfCanvasGroup.Toggle(evt.Visible, 0.1f);
            SidebarCanvasGroup.Toggle(evt.Visible, 0.2f);

            Conditional.Wait(0.2f)
                .Do(() =>
                {
                    SidebarSelectModuleGroup.enabled = evt.Visible;
                    MainSelectModuleGroup.enabled = evt.Visible;
                    
                    ShopController.ShopInventory.ToggleInventoryUI(evt.Visible);
                });
        }
        
        
        private void OnEquipModuleEvent(ShopModuleEquipEvent evt)
        {
            SidebarSelectModuleGroup.enabled = false;
            MainSelectModuleGroup.enabled = false;
                    
            ShopController.ShopInventory.ToggleInventoryUI(false);

            SidebarEquipModuleGroup.enabled = true;
            MainEquipModuleGroup.enabled = true;
        }

        private void OnExit()
        {
            using var evt = ToggleShopKeeperUIEvent.Get(false).SendGlobal();
        }

        private void OnDisable()
        {
            GEM.RemoveListener<ModulesLoadedEvent>(OnModulesLoaded);
            GEM.RemoveListener<ToggleShopKeeperUIEvent>(OnToggleShopKeeperUI);
            GEM.RemoveListener<ShopModuleEquipEvent>(OnEquipModuleEvent);
        }
    }
}