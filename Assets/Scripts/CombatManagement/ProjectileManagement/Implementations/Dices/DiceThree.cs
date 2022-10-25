using System;
using CharImplementations;
using CharImplementations.PlayerImplementation;
using UnityEngine;

namespace CombatManagement.ProjectileManagement.Implementations.Dices
{
    public class DiceThree : DuganDice
    {
        [SerializeField] 
        private Transform m_Wheels;

        private float m_RotateSpeed = 12f;
        
        protected bool m_Moving => Mover.Moving;
        
        public override void Initialize(Vector3 origin, Vector3 targetPos, float damage, CharType targetType, LayerMask layersToCollide,
            string layer)
        {
            Mover.Grounded = true;
            
            base.Initialize(origin, targetPos, damage, targetType, layersToCollide, layer);
        }
        
        public override void OnHit(Transform hitter)
        {
            if (hitter.TryGetComponent(out Player player))
            {
                player.OnImpact(TargetType, -ProjectileDamage);

                return;
            }

            if (hitter.TryGetComponent<IDamager>(out var damager))
            {
                var damage = (int)damager.Damage;
                
                if (m_Moving)
                    return;

                GetDamage(damage);
            }
        }

        private void FixedUpdate()
        {
            if(m_Moving)
                OnDash();
        }

        public void OnDash()
        {
            m_Wheels.Rotate(10 * m_RotateSpeed * Time.deltaTime, 0, 0);
        }

    }
}
