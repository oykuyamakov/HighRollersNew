using CharImplementations;
using CharImplementations.PlayerImplementation;
using Events;
using Misc.EventImplementations;
using Roro.Scripts.GameManagement;
using UnityCommon.Modules;
using UnityEngine;

namespace CombatManagement.ProjectileManagement.Implementations.Dices
{
    public class DiceOne : DuganDice
    {
        private bool m_DiceOnStop;

        private Conditional m_WaitConditional;
        
        public override void Initialize(Vector3 origin, Vector3 targetPos, float damage, CharType targetType, LayerMask layersToCollide,
            string layer)
        {
            Mover.Grounded = false;
            
            base.Initialize(origin, targetPos, damage, targetType, layersToCollide, layer);
            
            m_AnimationController.SetBool("Walk",true);
        }
        
        public override void OnHit(Transform hitter)
        {
            if (hitter.TryGetComponent(out Player _))
            {
                Stop();

                return;
            }

            if (!hitter.TryGetComponent<IDamager>(out var damager))
                return;

            var damage = (int)damager.Damage;
            GetDamage(damage);
            
            base.OnHit(hitter);
        }

        protected override void DeActivate()
        {
            m_DiceOnStop = false;
            m_WaitConditional?.Cancel();
            
            m_AnimationController.SetBool("Walk", false);

            base.DeActivate();
        }

        public override void DisableSelf()
        {
            m_DiceOnStop = false;
            m_WaitConditional?.Cancel();
            
            base.DisableSelf();
        }
        
        private void Stop()
        {
            if(m_DiceOnStop)
                return;

            m_DiceOnStop = true;
            
            m_AnimationController.SetBool("Walk", false);
            
            Mover.Reset();
            
            m_WaitConditional = Conditional.Wait(0.5f).Do(Blow);
        }

        protected void Blow()
        {
            using var evt2 = ParticleSpawnEvent.Get(ParticleType.SphereExplosion);
            evt2.SendGlobal();
            
            var part = evt2.Particle;
            part.Initialize(transform.position);
            part.transform.localScale = Vector3.one * 3;
                
            
            var layermask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("ProjectileEnemy");
            var hitColliders = Physics.OverlapSphere(transform.position, 3f, layermask);
            
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent<Player>(out var pl))
                {
                    pl.OnImpact(TargetType, -ProjectileDamage);
                    // TODO GET STUNNED?
                    pl.GetStunned(5f);
                }
                else if (hitCollider.transform.TryGetComponent<DiceOne>(out var pro))
                {
                    if(pro.m_UniqueId != m_UniqueId)
                       pro.Stop();

                }
            }
            
            Debug.Log("5");
            
                
            DeActivate();
        }
    }
}
