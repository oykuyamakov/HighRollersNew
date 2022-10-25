using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventoryManagement.UI
{
    public class InventoryItemUI : MonoBehaviour
    {
        public InventoryItem InventoryItem;
        
        public TextMeshProUGUI ItemName;
        public TextMeshProUGUI ItemDescription;
        public Image ItemIcon;

        public virtual void UpdateUI(InventoryItemData itemData)
        {
            ItemName.text = itemData.ItemName;
            ItemDescription.text = itemData.ItemDescription;
            ItemIcon.sprite = itemData.ItemIcon;
        }
    }
}