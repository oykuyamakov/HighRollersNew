using CombatManagement.ProjectileManagement;
using Events;
using Promises;
using UnityEngine;

namespace CombatManagement.EventImplementations
{
    public class ActivatePlayerProjectileThrowerEvent : Event<ActivatePlayerProjectileThrowerEvent>
    {
        public float MaxDistance;
        public ProjectileType ProjectileType;
        
       // public Promise<Projectile> ProjectilePromise;
        public Promise<Transform> ProjectilePromise;

        public static ActivatePlayerProjectileThrowerEvent Get(float maxDist, ProjectileType type)
        {
            var evt = GetPooledInternal();

            evt.MaxDistance = maxDist;
            evt.ProjectileType = type;
            evt.ProjectilePromise = Promise<Transform>.Create();
            
            return evt;
        }
    }
}