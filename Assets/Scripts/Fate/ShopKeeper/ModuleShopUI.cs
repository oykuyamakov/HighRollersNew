using Events;
using Fate.Modules;
using Fate.ShopKeeper.EventImplementations;
using InventoryManagement;
using InventoryManagement.UI;
using TMPro;
using UnityEngine.UI;

namespace Fate.ShopKeeper
{
    public class ModuleShopUI : InventoryItemUI
    {
        public TextMeshProUGUI CountText;
        
        public Button SelectButton;

        public ModuleShopItem ShopItem => InventoryItem as ModuleShopItem;

        private void OnEnable()
        {
            SelectButton.onClick.AddListener(OnSelected);
        }
        
        /// <summary>
        /// update ui with new item data
        /// </summary>
        public override void UpdateUI(InventoryItemData itemData)
        {
            InventoryItem?.RemoveListener<UpdateShopItemUIEvent>(UpdateUI);
            
            base.UpdateUI(itemData);

            CountText.text = ShopItem.Count.ToString();
            
            InventoryItem.AddListener<UpdateShopItemUIEvent>(UpdateUI);
        }

        /// <summary>
        /// update with existing data, for count text and stuff
        /// </summary>
        public void UpdateUI(UpdateShopItemUIEvent evt)
        {
            CountText.text = ShopItem.Count.ToString();
        }
        
        private void OnSelected()
        {
            using var evt = ShopModuleSelectionEvent.Get(ShopItem).SendGlobal();
            
            // GEM.AddListener<UpdateShopItemUIEvent>(UpdateUI);
        }
        
        private void OnDisable()
        {
            SelectButton.onClick.RemoveListener(OnSelected);
        }
    }
}