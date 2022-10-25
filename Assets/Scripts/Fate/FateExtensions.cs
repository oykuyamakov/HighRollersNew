using System;
using CombatManagement.ProjectileManagement;
using Events;
using Fate.EventImplementations;
using Fate.Modules;
using Fate.PassiveSkills;
using Misc;
using Misc.EventImplementations;
using Roro.Scripts.GameManagement;
using UnityCommon.Runtime.Extensions;
using UnityEngine;

namespace Fate
{
    public static class FateExtensions
    {
        // TODO: open for improvement
        public static void EquipModule(this ModuleRuntimeData module)
        {
            using var evt = ModuleEquipmentEvent.Get(module).SendGlobal();
        }
        
        public static Transform GetClosestEnemy(int count, Collider[] enemies, Vector3 playerPos)
        {
            Transform bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = playerPos;

            for (var i = 0; i < count; i++)
            {
                // TODO: for deactive enemies
                if (enemies[i].TryGetComponent<Mover>(out var mover))
                {
                    if (!mover.Enabled)
                        continue;
                }

                if (enemies[i].TryGetComponent<Projectile>(out var projectile))
                {
                    if (!projectile.HasHealth)
                        continue;
                }

                var t = enemies[i].transform;

                Vector3 directionToTarget = t.position.WithY(currentPosition.y) - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = t;
                }
            }

            return bestTarget;
        }

        public static int GetNearEnemies(Transform playerT, ref Collider[] overlappingEnemies, float radius)
        {
            var playerPos = playerT.transform.position;

            LayerMask enemyLayerMask =
                (1 << LayerMask.NameToLayer("Enemy")
                 | 1 << LayerMask.NameToLayer("ProjectileEnemy"));

            int overlappingCont = Physics.OverlapSphereNonAlloc(playerPos, radius, overlappingEnemies, enemyLayerMask);

            return overlappingCont;
        }

        public static Particle GetParticle(ParticleType type, Transform parent, bool selfDisable = true)
        {
            Particle particle;

            using var evt = ParticleSpawnEvent.Get(type);
            {
                evt.SendGlobal();

                particle = evt.Particle;
            }

            particle.Initialize(parent, selfDisable);

            return particle;
        }

        public static Particle GetParticle(ParticleType type, Vector3 position)
        {
            Particle particle;

            using var evt = ParticleSpawnEvent.Get(type);
            {
                evt.SendGlobal();

                particle = evt.Particle;
            }

            particle.Initialize(position);

            return particle;
        }
    }
}