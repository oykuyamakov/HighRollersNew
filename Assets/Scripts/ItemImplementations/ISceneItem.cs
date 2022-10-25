using UnityEngine;

namespace ItemImplementations
{
    public interface ISceneItem
    {
        public void GetDamage();
        
        public int Health { get; set; }

        public void Die();

        public void Place(Vector3 position);
    }
}
