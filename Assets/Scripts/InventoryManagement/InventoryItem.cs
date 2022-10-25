namespace InventoryManagement
{
    public abstract class InventoryItem
    {
        public abstract InventoryItemData GetData();

        public abstract void Load(object arg);
        
        public abstract void Save(object arg);
    }

    public abstract class InventoryItem<T> : InventoryItem
    {
    }
}