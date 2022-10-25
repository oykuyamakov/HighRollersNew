using Events;
using Fate;
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

            TierOutline.color = FateSettings.Get().RarityColorRefs[(int)data.Tier];

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
            ModuleInventoryItem.ModuleData.EquipModule();
        }

        private void OnDisable()
        {
            SelectButton.onClick.RemoveListener(OnSelected);
            InventoryItem.RemoveListener<InventoryItemModifiedEvent>(OnItemModified);
        }
    }
}