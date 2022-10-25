using CharImplementations;
using CharImplementations.EnemyImplementations;
using CharImplementations.PlayerImplementation;
using CombatManagement.ProjectileManagement;
using Events;
using Misc.EventImplementations;
using Roro.Scripts.GameManagement;
using UnityCommon.Modules;
using UnityEngine;
using Utility.Extensions;

namespace Fate.Modules
{
    public class Fuse : Module
    {
        private float m_KnockbackStrength;
        private Player m_Player;

        private Collider[] m_EnemiesInRange = new Collider[25];

        public override void OnActiveSkill(ModuleRuntimeData runtimeData)
        {
            m_KnockbackStrength = runtimeData.GetData(0);
            m_Player = PlayerExtensions.GetPlayer();
            var playerT = m_Player.transform;

            KnockbackEnemies(playerT);

            FateExtensions.GetParticle(ParticleType.Fuse, playerT.position);
        }

        private void KnockbackEnemies(Transform playerT)
        {
            var resultCount = FateExtensions.GetNearEnemies(playerT, ref m_EnemiesInRange, 50f);

            for (var i = 0; i < resultCount; i++)
            {
                if (m_EnemiesInRange[i].transform.TryGetComponent<Enemy>(out var enemy))
                {
                    enemy.GetKnocked(0.5f, -enemy.transform.forward * m_KnockbackStrength);
                }
                else if (m_EnemiesInRange[i].transform.TryGetComponent<Projectile>(out var projectile))
                {
                    if (projectile.HasHealth)
                    {
                        projectile.GetKnocked(0.5f, -projectile.transform.forward * m_KnockbackStrength);
                    }
                    else
                    {
                        projectile.DisableSelf();
                    }
                }
            }
        }
    }

    // TODO: this implementation was misunderstood, but i will keep it here for future
    // public class Fuse : ProjectileModule
    // {
    //     private float m_KnockbackStrength;
    //     private float m_Duration;
    //     
    //     public override void OnActiveSkill(ModuleRuntimeData runtimeData)
    //     {
    //         m_KnockbackStrength = runtimeData.GetAdditionalData(0);
    //         m_Duration = runtimeData.GetDurationData();
    //     }
    //
    //     public override void OnThrown(Transform indicator)
    //     {
    //         
    //     }
    //     
    //     public override void OnThrown(Transform indicator)
    //     {
    //         var position = indicator.position;
    //         var direction = -indicator.forward;
    //     
    //         var targetPosition = position + (direction * ProjectileSpeed * m_Duration);
    //     
    //         using (var getProjectileEvt = GetProjectileEvent.Get(ProjectileType.Fuse))
    //         {
    //             getProjectileEvt.SendGlobal();
    //     
    //             var fuse = getProjectileEvt.Projectile;
    //     
    //             fuse.transform.localScale = m_PlaceableModuleData.Scale;
    //     
    //             fuse.Initialize(position, -direction, m_KnockbackStrength, CharType.Enemy, Physics.AllLayers,
    //                 "ProjectilePlayer");
    //     
    //             var projectileInfo = new ProjectileInfo();
    //     
    //             projectileInfo.MovementType = ProjectileInfo.ProjectileMovementType.LinearMove;
    //             projectileInfo.Damage = m_KnockbackStrength;
    //             
    //             fuse.Mover.MoveToPos(targetPosition, ProjectileSpeed);
    //         }
    //     }
    // }
}