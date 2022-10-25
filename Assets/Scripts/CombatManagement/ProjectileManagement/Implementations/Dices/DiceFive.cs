using CharImplementations;
using CharImplementations.PlayerImplementation;
using CombatManagement.EventImplementations;
using Events;
using Roro.UnityCommon.Scripts.Runtime.Utility;
using UnityEngine;

namespace CombatManagement.ProjectileManagement.Implementations.Dices
{
    public class DiceFive : DuganDice
    {
        private IAction m_FireAction;

        private LayerMask m_FireLayerMask;

        private float m_DodgeDuration = 0.2f;
        
        private bool m_Firing;

        private int m_DodgeCount;

        protected override void Awake()
        {
            base.Awake();
            
            m_FireLayerMask = 1 <<  LayerMask.NameToLayer("Obstacle")
                              | 1 <<  LayerMask.NameToLayer("Player") 
                              | 1 << LayerMask.NameToLayer("ProjectilePlayer");
        }

        public override void Initialize(Vector3 origin, Vector3 targetPos, float damage, CharType targetType, LayerMask layersToCollide,
            string layer)
        {
            base.Initialize(origin, targetPos, damage, targetType, layersToCollide, layer);
            
            
            m_FireAction = new ArbitraryAction(SendBullet, m_PhaseTwoValues.Dice5AttackValues);

        }
        
        public override void OnHit(Transform hitter)
        {
            if (hitter.TryGetComponent(out Player player))
            {
                player.OnImpact(TargetType, -ProjectileDamage);

                return;
            }

            if (!hitter.TryGetComponent<IDamager>(out var damager)) 
                return;
            
            if (m_DodgeCount < 3)
            {
                Mover.SnuckJumpMovement(transform.forward * 2, 4, 1, m_DodgeDuration);
                m_DodgeCount++;
                return;
            }
            
            var damage = (int)damager.Damage;
            GetDamage(damage);
        }
        
        private void Update()
        {
            m_FireAction?.Update(Time.deltaTime);
        }
        
        public override void DisableSelf()
        {
            m_FireAction = null;
            m_Firing = false;
            
            base.DisableSelf();
        }

        protected override void DeActivate()
        {
            m_FireAction = null;
            m_Firing = false;
            
            base.DeActivate();
        }
        
        private void SendBullet()
        {
            using var evt = GetProjectileEvent.Get(ProjectileType.Dice5Projectile);
            evt.SendGlobal();
            
            var bullet = evt.Projectile;

            var pos = Player.PlayerTransform.position + Player.PlayerTransform.forward * 2;
            bullet.Initialize(transform.position, pos, ProjectileDamage, CharType.Player, m_FireLayerMask, "ProjectileEnemy");
            bullet.Mover.MoveToPos(pos,m_PhaseTwoValues.Dice5BulletSpeed);
        }
    }
}
