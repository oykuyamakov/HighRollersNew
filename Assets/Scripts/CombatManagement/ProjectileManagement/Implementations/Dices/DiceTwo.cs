using System.Linq;
using CharImplementations;
using CharImplementations.PlayerImplementation;
using Events;
using UnityCommon.Modules;
using UnityEngine;

namespace CombatManagement.ProjectileManagement.Implementations.Dices
{
    public class DiceTwo : DuganDice
    {
        private bool m_SearchPlayer;
        private bool m_Jumping;
        
        private Conditional m_CooldownConditional;
        private float m_JumpCooldown => m_PhaseTwoValues.JumpCooldown;
        private float m_JumpDuration => m_PhaseTwoValues.JumpDuration;
        private float m_CheckDiameter => m_PhaseTwoValues.ScanAreaScale;

        public override void Initialize(Vector3 origin, Vector3 targetPos, float damage, CharType targetType, LayerMask layersToCollide,
            string layer)
        {
            Mover.Grounded = false;
            
            base.Initialize(origin, targetPos, damage, targetType, layersToCollide, layer);

            m_AnimationController.SetBool("Walk", true);
                
            m_SearchPlayer = true;
        }
        
        public override void OnHit(Transform hitter)
        {
            if(m_Jumping)
                return;
            
            if (!hitter.TryGetComponent<IDamager>(out var damager))
                return;

            var damage = (int)damager.Damage;
            GetDamage(damage);
            
            base.OnHit(hitter);
        }

        protected override void DeActivate()
        {
            m_SearchPlayer = false;
            m_Jumping = false;
            m_CooldownConditional?.Cancel();
            
            m_AnimationController.SetBool("Walk", false);

            base.DeActivate();
        }

        public override void DisableSelf()
        {
            m_SearchPlayer = false;
            m_Jumping = false;
            m_CooldownConditional?.Cancel();
            
            base.DisableSelf();
        }

        private void FixedUpdate()
        {
            CheckForPlayer();
        }

        private void CheckForPlayer()
        {
            if(!m_SearchPlayer)
                return;
            
            if(m_Jumping)
                return;
            
            Collider[] hitDetects = Physics.OverlapSphere(transform.position, m_CheckDiameter, 1 << LayerMask.NameToLayer("Player"));

            if(!hitDetects.Any())
                return;
            
            var vector1 = transform.position;
            var vector2 = hitDetects[0].transform.position;
            // translate the vector1 to %60 of the vector2
            var vector3 = Vector3.Lerp(vector1, vector2, 0.8f);

            TryCatchPlayer(vector3);
        }

        private void TryCatchPlayer(Vector3 pos)
        {
            if(m_Jumping)
                return;
            
            m_Jumping = true;
            m_AnimationController.Trigger("Jump");

            using var evt = SoundPlayEvent.Get(m_PhaseTwoValues.Dice2JumpSound);
            evt.SendGlobal();
            
            Mover.SnuckJumpMovement(pos, Random.Range(5,8), 1, m_JumpDuration, TryStun);
        }

        // TODO non alloc
        private void TryStun()
        {
            Collider[] hitDetects = Physics.OverlapSphere(transform.position, 3f, 1 << LayerMask.NameToLayer("Player"));
            
            foreach (var t in hitDetects)
            {
                var h = t.transform;
                if (!h.TryGetComponent<Player>(out var pl)) 
                    continue;
                
                pl.OnImpact(TargetType, -ProjectileDamage);
                pl.GetStunned(5f);
            }

            m_CooldownConditional = Conditional.Wait(m_JumpCooldown).Do(() => m_Jumping = false);
            //m_TryStunning = false;
        }

    }
}
