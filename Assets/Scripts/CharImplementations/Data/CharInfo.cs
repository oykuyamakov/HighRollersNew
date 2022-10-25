using UnityEngine;
using UnityEngine.Serialization;

namespace CharImplementations.Data
{
    [CreateAssetMenu(fileName = "New Char", menuName = "Char/Char")]
    public class CharInfo : ScriptableObject
    {
        public GameObject Prefab;
        public string Name;
        public float HealthMax;
        public float MovementSpeed;
    }
}
