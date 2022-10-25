using CharImplementations.PlayerImplementation;
using CombatManagement.EventImplementations;
using CombatManagement.ProjectileManagement;
using Events;
using UnityCommon.Runtime.Extensions;
using UnityCommon.Runtime.Utility;
using UnityEngine;

namespace CharImplementations.EnemyImplementations.ZombieImplementations
{
    public class Thrower : Zombie
    {
        private void Start()
        {
            AnimController.SetInt(HASH_ZOMBIE_ID, 1);
        }

        protected override void Update()
        {
            base.Update();
        }

        public override void RiseFromTheDead()
        {
            base.RiseFromTheDead();

            FireAction = new TimedAction(Attack, 0f, 5f);
            SetMovementTarget(ZombieTarget.Player);
        }

        public override void StartAttack()
        {
            base.StartAttack();
        }

        public override void Attack()
        {
            using var evt = GetProjectileEvent.Get(ProjectileType.ZombieProjectile);
            {
                evt.SendGlobal();

                var playerPos = m_PlayerTransform.position;

                var newProjectile = evt.Projectile;
                newProjectile.Initialize(transform.position, playerPos, ZombieData.DamagePerSecond,
                    CharType.Player, Physics.AllLayers, "ProjectileEnemy");

                var projectileInfo = new ProjectileInfo();

                projectileInfo.MovementType = ProjectileInfo.ProjectileMovementType.OrganicJump;
                projectileInfo.Damage = ZombieData.DamagePerSecond;

                newProjectile.Mover.MoveToPos(playerPos.WithY(Player.GlobalProjectileY), 10f);
            }
        }

        public override void StopAttack()
        {
            base.StopAttack();
        }
    }
}