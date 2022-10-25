using Fate.Modules;
using UnityEngine;

namespace InventoryManagement
{
    public class InventoryItemData
    {
        public string ItemName;
        public string ItemDescription;
        public Sprite ItemIcon;

        public InventoryItemData(string name, string desc, Sprite icon)
        {
            ItemName = name;
            ItemDescription = desc;
            ItemIcon = icon;
        }
    }

    public class ModuleInventoryData : InventoryItemData
    {
        public ModuleTier Tier;
        
        public ModuleInventoryData(string name, string desc, Sprite icon, ModuleTier tier) : base(name, desc, icon)
        {
            Tier = tier;
        }
    }
    
    public class ModuleShopData : InventoryItemData
    {
        public int Count;
        
        public ModuleShopData(string name, string desc, Sprite icon, int count) : base(name, desc, icon)
        {
            Count = count;
        }
    }
}