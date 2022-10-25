using CharImplementations;
using CombatManagement.ProjectileManagement;
using Events;
using UnityEngine;

namespace CombatManagement.EventImplementations
{
    public class GetProjectileEvent : Event<GetProjectileEvent>
    {
        public ProjectileType ProjectileType;
        public Projectile Projectile;

        public static GetProjectileEvent Get(ProjectileType type)
        {
            var evt = GetPooledInternal();
            evt.ProjectileType = type;
            return evt;
        }
    }
}
