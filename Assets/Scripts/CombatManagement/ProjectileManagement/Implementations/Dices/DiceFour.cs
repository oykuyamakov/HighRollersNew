using CharImplementations;
using CharImplementations.PlayerImplementation;
using Roro.UnityCommon.Scripts.Runtime.Utility;
using UnityCommon.Modules;
using UnityCommon.Runtime.Extensions;
using UnityEngine;

namespace CombatManagement.ProjectileManagement.Implementations.Dices
{
    public class DiceFour : DuganDice
    {
        [SerializeField]
        private LineRenderer m_LineRenderer;

        [SerializeField]
        private Transform m_Drill;
        
        private IAction m_FireAction;
        
        private bool m_BeamStarted;
        private bool m_Firing;
        
        private float m_DrillRotateSpeed = 20;

        private Vector3 m_PlayerPosOnBeamStart;
        
        private LayerMask m_FireLayerMask;
        private float m_T;

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
            
            
            m_FireAction = new CooldownedAction(SendLaser,
                m_PhaseTwoValues.BeamInitialDelay, m_PhaseTwoValues.BeamCoolDownDuration,
                m_PhaseTwoValues.BeamFireDuration, OnLaserCoolDown);

            m_LineRenderer.transform.localScale /= m_ScaleMultiplier;

            GetDrilled();
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
            m_LineRenderer.enabled = false;
            m_Firing = false;
            
            base.DisableSelf();
        }

        protected override void DeActivate()
        {
            m_FireAction = null;
            m_LineRenderer.enabled = false;
            m_Firing = false;
            
            base.DeActivate();
        }

        private void GetDrilled()
        {
            m_AnimationController.SetBool("Drill", true);

            Conditional.For(m_PhaseTwoValues.BeamInitialDelay).Do(() =>
            {
                m_Drill.transform.Rotate(0, 100 * m_DrillRotateSpeed * Time.deltaTime, 0);
            }).OnComplete(() =>
            {
                m_AnimationController.SetBool("Drill", false);
            });
        }
        
        private void SendLaser()
        {
            if (!m_BeamStarted)
            {
                m_PlayerPosOnBeamStart = Player.PlayerTransform.position;
                m_T = 0;
            }
            
            m_BeamStarted = true;

            m_LineRenderer.enabled = true;
            m_LineRenderer.positionCount = 2;
            m_LineRenderer.transform.rotation = Quaternion.identity;

            m_T += m_PhaseTwoValues.BeamTrackSpeed * Time.deltaTime;

            var targetPos = Vector3.Lerp(m_PlayerPosOnBeamStart, Player.PlayerTransform.position, m_T);

            var hitPos = FireManager.FireRay(transform.position,
                targetPos, 100, ProjectileDamage, CharType.Player, m_FireLayerMask, transform);
            
            transform.forward = hitPos.WithY(Player.GlobalProjectileY) - transform.position.WithY(Player.GlobalProjectileY);
            var posX = hitPos.x - transform.position.x;

            m_LineRenderer.SetPositions(new[] { Vector3.zero, new Vector3(posX, 0, hitPos.z - transform.position.z) });

            m_Firing = true;
        }
        
        private void OnLaserCoolDown()
        {
            m_LineRenderer.enabled = false;
            m_Firing = false;
            m_BeamStarted = false;
        }
    }
}
