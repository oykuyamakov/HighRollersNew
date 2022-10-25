using CharImplementations;
using CombatManagement.WeaponImplementation;
using UnityEngine;

namespace CombatManagement.ProjectileManagement
{
    public interface IProjectile
    { 
        public ProjectileType ProjectileType { get; }
        public CharType TargetType { get; set; }
        public LayerMask TargetLayers { get; set; }
        public float ProjectileDamage { get; set; }
        public Transform SelfTransform();
        public Collider SelfCollider();
        public void Initialize(Vector3 origin, Vector3 target, float damage, CharType targetType, LayerMask layersToCollide, string selfLayer);
    }
}