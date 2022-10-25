using Sirenix.OdinInspector;
using UnityEngine;

namespace Fate.Modules.Data
{
    [CreateAssetMenu(menuName = "Fate/Modules/Placeable Data")]
    public class PlaceableModuleData : ModuleData
    {
        [BoxGroup("Placeable Object")]
        public GameObject PlaceableObjectPrefab;

        [BoxGroup("Placeable Object")]
        public bool UseIndicator;
        
        [ShowIf("@UseIndicator")]
        [BoxGroup("Placeable Object")]
        public Sprite PlaceableIndicator;
        
        [BoxGroup("Placeable Object")]
        public Vector3 Scale = Vector3.one;

        [BoxGroup("Placeable Object")]
        public float MaxPlaceableDistance = 10f; // TODO: might need to be adjusted

        [BoxGroup("Placeable Object")]
        public bool IsProjectile;

        [ShowIf("@IsProjectile")]
        [BoxGroup("Placeable Object")]
        public float ProjectileSpeed = 100f;
    }
}