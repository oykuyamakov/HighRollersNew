using Events;
using Fate.Modules;
using Fate.ShopKeeper.EventImplementations;
using InventoryManagement;
using InventoryManagement.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility.Extensions;

namespace Fate.ShopKeeper
{
    public class ModuleShopUI : InventoryItemUI
    {
        public ModuleShopItem ShopItem => InventoryItem as ModuleShopItem;

        public bool Owned => ShopItem.OwnedCount > 0;
        
        public TextMeshProUGUI CountText;

        public CanvasGroup RegularButtonsGroup;
        public CanvasGroup OwnedButtonsGroup;

        public Button SelectButton;
        
        public Button BuyButton;
        public Button TryButton;

        public Button EquipButton;
        public Button RebuyButton;

        public RectTransform AnimationArea;

        private void OnEnable()
        {
            SelectButton.onClick.AddListener(OnSelected);
            BuyButton.onClick.AddListener(OnBuy);
            TryButton.onClick.AddListener(OnTry);
            EquipButton.onClick.AddListener(OnEquip);
            RebuyButton.onClick.AddListener(OnBuy);
        }
        
        /// <summary>
        /// update ui with new item data
        /// </summary>
        public override void UpdateUI(InventoryItemData itemData)
        {
            InventoryItem?.RemoveListener<UpdateShopItemUIEvent>(UpdateUI);
            
            ItemName.text = itemData.ItemName;
            CountText.text = $"{ShopItem.OwnedCount.ToString()} OWNED";

            OwnedButtonsGroup.Toggle(Owned, 0);
            RegularButtonsGroup.Toggle(!Owned, 0);

            InventoryItem.AddListener<UpdateShopItemUIEvent>(UpdateUI);
        }

        /// <summary>
        /// update with existing data, for count text and stuff
        /// </summary>
        public void UpdateUI(UpdateShopItemUIEvent evt)
        {
            CountText.text = $"{ShopItem.OwnedCount.ToString()} OWNED";
            
            OwnedButtonsGroup.Toggle(Owned, 0);
            RegularButtonsGroup.Toggle(!Owned, 0);
        }
        
        private void OnSelected()
        {
            using var evt = ShopModuleSelectionEvent.Get(ShopItem).SendGlobal();
        }
        
        private void OnBuy()
        {
            using var evt = ShopModuleBuyEvent.Get(ShopItem).SendGlobal();
        }
        
        private void OnEquip()
        {
            using var evt = ShopModuleEquipEvent.Get(ShopItem).SendGlobal();
        }

        private void OnTry()
        {
            using var evt = ShopModuleTryEvent.Get(ShopItem).SendGlobal();
        }
        
        private void OnDisable()
        {
            SelectButton.onClick.RemoveListener(OnSelected);
            BuyButton.onClick.RemoveListener(OnBuy);
            TryButton.onClick.RemoveListener(OnTry);
            EquipButton.onClick.RemoveListener(OnEquip);
            RebuyButton.onClick.RemoveListener(OnBuy);
        }
    }
}