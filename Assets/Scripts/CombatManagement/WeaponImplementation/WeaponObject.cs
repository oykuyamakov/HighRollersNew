using Sirenix.OdinInspector;
using UnityEngine;

namespace CombatManagement.WeaponImplementation
{
    [CreateAssetMenu(menuName = "Weapon", fileName = "New Weapon")]
    public class WeaponObject : ScriptableObject
    {
        //public GameObject Prefab;
        public WeaponData WeaponData;
    }
}
