using Events;
using Fate.EventImplementations;
using Fate.Modules;
using InventoryManagement.EventImplementations;
using InventoryManagement.UI;
using UnityEngine;
using UnityEngine.UI;

namespace InventoryManagement.InventoryImplementations.Modules
{
    public class ModuleInventoryUI : InventoryItemUI
    {
        public Image TierOutline;

        public Button SelectButton;

        public ModuleInventoryItem ModuleInventoryItem => InventoryItem as ModuleInventoryItem;

        private void OnEnable()
        {
            SelectButton.onClick.AddListener(OnSelected);
            InventoryItem.AddListener<InventoryItemModifiedEvent>(OnItemModified);
        }

        public override void UpdateUI(InventoryItemData itemData)
        {
            base.UpdateUI(itemData);

            ModuleInventoryData data = itemData as ModuleInventoryData;

            // TODO: dummy shit to test functionality
            switch (data.Tier)
            {
                case ModuleTier.Uncommon:
                    TierOutline.color = Color.green;
                    break;

                case ModuleTier.Rare:
                    TierOutline.color = Color.blue;
                    break;

                case ModuleTier.Epic:
                    TierOutline.color = Color.red;
                    break;

                case ModuleTier.Legendary:
                    TierOutline.color = new Color(1, 0.5f, 0f);
                    break;

                default:
                    TierOutline.color = Color.grey;
                    break;
            }

            ToggleEquipped(ModuleInventoryItem.ModuleData.Equipped);
        }

        private void OnItemModified(InventoryItemModifiedEvent evt)
        {
            ToggleEquipped(ModuleInventoryItem.ModuleData.Equipped);
        }

        private void ToggleEquipped(bool equipped)
        {
            ItemIcon.color = equipped ? Color.clear : Color.white;
        }

        protected virtual void OnSelected()
        {
            using var evt = ModuleEquipmentEvent.Get(ModuleInventoryItem.ModuleData).SendGlobal();
        }

        private void OnDisable()
        {
            SelectButton.onClick.RemoveListener(OnSelected);
            InventoryItem.RemoveListener<InventoryItemModifiedEvent>(OnItemModified);
        }
    }
}