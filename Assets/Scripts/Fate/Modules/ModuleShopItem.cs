using InventoryManagement;

namespace Fate.Modules
{
    public class ModuleShopItem : ModuleInventoryItem
    {
        public int Count;
        public int OwnedCount => ModuleManager.OwnedModules[ModuleData.Module].Count;
        
        public override InventoryItemData GetData()
        {
            return new ModuleShopData(ModuleData.Data.ModuleName,
                ModuleData.Data.ModuleDescription, ModuleData.Data.ModuleIcon, Count);
        }

        public override void Load(object arg)
        {
            // TODO: not implemented
        }

        public override void Save(object arg)
        {
            // TODO: not implemented
        }
    }
}