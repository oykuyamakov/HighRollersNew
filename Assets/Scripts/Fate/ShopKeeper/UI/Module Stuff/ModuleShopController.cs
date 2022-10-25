using System;
using System.Collections.Generic;
using Events;
using Fate.EventImplementations;
using Fate.Modules;
using Fate.ShopKeeper.EventImplementations;
using InventoryManagement;
using Promises;
using ResourceManagement;
using ResourceManagement.EventImplementations;
using UnityEngine;

namespace Fate.ShopKeeper
{
    public class ModuleShopController : MonoBehaviour
    {
        public Dictionary<ModuleCategory, List<InventoryItem>> ModuleShopItemsByCategory =
            new Dictionary<ModuleCategory, List<InventoryItem>>();

        public ModuleCategory SelectedCategory;

        public InventorySystem ShopInventory;

        public DiceSelectionController diceSelectionController;

        private ModuleShopItem m_SelectedModule;

        private Promise<int> m_DiceSelectionPromise;
        private Promise<bool> m_CurrentCategoryPromise;

        private void OnEnable()
        {
            ShopInventory.Index = InventoryId.ShopKeeper;

            GEM.AddListener<ShopModuleSelectionEvent>(OnModuleSelection);
            GEM.AddListener<ShopModuleBuyEvent>(OnTryBuyModule);
        }

        public Promise<bool> SetCategory(ModuleCategory category)
        {
            if (Promise<bool>.IsValid(m_CurrentCategoryPromise))
            {
                m_CurrentCategoryPromise.Complete(true);
            }

            m_CurrentCategoryPromise = Promise<bool>.Create();

            SelectedCategory = category;
            Refresh();

            return m_CurrentCategoryPromise;
        }

        private void InitializeCategoryDict()
        {
            foreach (ModuleCategory category in (ModuleCategory[])Enum.GetValues(typeof(ModuleCategory)))
            {
                ModuleShopItemsByCategory.Add(category, new List<InventoryItem>() { });
            }
        }

        public void FillInventory(List<Module> modules)
        {
            InitializeCategoryDict();

            for (var i = 0; i < modules.Count; i++)
            {
                ModuleRuntimeData runtimeData = new ModuleRuntimeData();

                runtimeData.Module = ModuleManager.Modules[i];

                ModuleShopItem item = new ModuleShopItem();

                item.ModuleData = runtimeData;
                item.Count = 3;

                ModuleShopItemsByCategory[runtimeData.Data.Category].Add(item);

                ShopInventory.AddToInventory(item);
            }

            ShopInventory.SendModifiedEvent();
        }

        public void Refresh()
        {
            ShopInventory.SetItems(ModuleShopItemsByCategory[SelectedCategory]);
        }

        private void OnModuleSelection(ShopModuleSelectionEvent evt)
        {
            m_SelectedModule = evt.SelectedModule;
        }

        private void OnTryBuyModule(ShopModuleBuyEvent evt)
        {
            m_SelectedModule = evt.Module;

            OpenDiceSelectionPopup();
        }

        public void OpenDiceSelectionPopup()
        {
            var promise = diceSelectionController.Instantiate(m_SelectedModule.ModuleData.Data);
            promise.OnResultT += (b, i) => OnDiceSelectionComplete(b, i);
        }


        // TODO: needs fixing
        private void OnDiceSelectionComplete(bool success, int totalRolledAmt)
        {
            if (!success)
            {
                m_DiceSelectionPromise.Release();
                return;
            }

            using var spendResourceEvt = SpendResourceEvent.Get(ResourceType.Dice, totalRolledAmt);

            m_SelectedModule.ModuleData.Tier = ModuleManager.GetTierForModule(totalRolledAmt);

            using var addToInventoryEvt = AddModuleToInventoryEvent.Get(m_SelectedModule.ModuleData).SendGlobal();

            UpdateShopItem();

            m_DiceSelectionPromise.Release();
        }

        private void UpdateShopItem()
        {
            m_SelectedModule.Count--;

            if (m_SelectedModule.Count <= 0)
            {
                ModuleShopItemsByCategory[SelectedCategory].Remove(m_SelectedModule);
                ShopInventory.RemoveFromInventory(m_SelectedModule);
                ShopInventory.SendModifiedEvent();
                return;
            }

            var newRuntimeData = new ModuleRuntimeData();

            newRuntimeData.Module = m_SelectedModule.ModuleData.Module;

            m_SelectedModule.ModuleData = newRuntimeData;

            using (var updateItemEvt = UpdateShopItemUIEvent.Get())
            {
                m_SelectedModule.SendEvent(updateItemEvt);
            }
        }

        private void OnDisable()
        {
            GEM.RemoveListener<ShopModuleSelectionEvent>(OnModuleSelection);
            GEM.RemoveListener<ShopModuleBuyEvent>(OnTryBuyModule);
        }
    }
}