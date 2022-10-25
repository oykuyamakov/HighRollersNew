using CharImplementations.Data;
using UnityEngine;

namespace CharImplementations.EnemyImplementations.ZombieImplementations
{
    public enum ZombieType
    {
        Holder,
        Thrower,
        Runner,
        Chonker
    }
    
    [CreateAssetMenu(menuName = ("Char/Enemy/Zombie Data"))]
    public class ZombieData : CharInfo
    {
        public ZombieType Type;
        
        public int DamagePerSecond;
        public int DoorDamagePerSecond;
        public float Range;
        
        public float AttackRange;
    }
}