using UnityEngine;

namespace ItemImplementations
{
    public interface IInventoryItem
    {
        public void Preview();

        public Sprite PreviewImage();

        public void Use();

        public void Equip();

        public void UnEquip();
    }
}
