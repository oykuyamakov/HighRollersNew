using Events;
using Misc.EventImplementations;
using Roro.Scripts.GameManagement;
using UnityEngine;

namespace CombatManagement.ProjectileManagement.Implementations
{
    public class Missile : Projectile
    {
        public override void DisableSelf()
        {
            using var evt = ParticleSpawnEvent.Get(ParticleType.MissileExplosion);
            evt.SendGlobal();
            var particle = evt.Particle;
            particle.Initialize(transform.position);
            
            base.DisableSelf();
            
        }
    }
}
