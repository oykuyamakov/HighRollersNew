using InventoryManagement;
using UnityEngine;

namespace Fate.Modules
{
    public class ModuleInventoryItem : InventoryItem
    {
        [SerializeReference]
        public ModuleRuntimeData ModuleData;

        public override InventoryItemData GetData()
        {
            return new ModuleInventoryData(ModuleData.Data.ModuleName,
                ModuleData.Data.ModuleDescription, ModuleData.Data.ModuleIcon, ModuleData.Tier);
        }
        
        public override void Load(object arg)
        {
            var loadIndex = (int) arg;
            ModuleData.Load(loadIndex);
        }

        public override void Save(object arg)
        {
            var saveIndex = (int) arg;
            ModuleData.Save(saveIndex);
        }
    }
}