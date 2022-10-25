using Events;
using Fate.EventImplementations;
using Fate.Modules;
using Fate.ShopKeeper.EventImplementations;
using InventoryManagement;
using Promises;
using ResourceManagement;
using ResourceManagement.EventImplementations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Fate.ShopKeeper
{
    public class ModuleShopController : MonoBehaviour
    {
        public InventorySystem ShopInventory;

        public DiceSelectionPopup DiceSelectionPopup;

        private Promise<int> m_Promise;

        private ModuleShopItem m_SelectedModule;
        
        private void OnEnable()
        {
            ShopInventory.Index = InventoryId.ShopKeeper;
            
            GEM.AddListener<ShopModuleSelectionEvent>(OnModuleSelection);
        }

        // TODO: filling with dummy data
        [Button]
        public bool FillInventory()
        {
            if (ModuleManager.Modules.Count == 0)
                return false;
            
            for (var i = 0; i < ModuleManager.Modules.Count; i++)
            {
                ModuleRuntimeData runtimeData = new ModuleRuntimeData();

                runtimeData.Module = ModuleManager.Modules[i];
                
                ModuleShopItem item = new ModuleShopItem();

                item.ModuleData = runtimeData;
                item.Count = 3;
                
                ShopInventory.AddToInventory(item);
            }
            
            ShopInventory.SendModifiedEvent();

            return true;
        }
        
        private void OnModuleSelection(ShopModuleSelectionEvent evt)
        {
            m_SelectedModule = evt.SelectedModule;
            OpenDiceSelectionPopup();
        }

        public void OpenDiceSelectionPopup()
        {
            var promise = DiceSelectionPopup.Instantiate();
            promise.OnResultT += (b, i) =>  OnDiceSelectionComplete(b, i);
        }

        private void OnDiceSelectionComplete(bool success, int selectedDiceCount)
        {
            if (!success)
            {
                m_Promise.Release();
                return;
            }
            
            using var spendResourceEvt = SpendResourceEvent.Get(ResourceType.Dice, selectedDiceCount);
            
            m_SelectedModule.ModuleData.Tier = ModuleManager.GetTierForModule(selectedDiceCount);
            
            using var addToInventoryEvt = AddModuleToInventoryEvent.Get(m_SelectedModule.ModuleData).SendGlobal();
            
            UpdateShopItem();

            m_Promise.Release();
        }

        // TODO: maybe not here
        private void UpdateShopItem()
        {
            m_SelectedModule.Count--;
            
            if (m_SelectedModule.Count <= 0)
            {
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
        }
    }
}