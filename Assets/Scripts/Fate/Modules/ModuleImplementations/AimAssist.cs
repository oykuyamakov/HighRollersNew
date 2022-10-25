using CharImplementations;
using CharImplementations.PlayerImplementation;
using CombatManagement.WeaponImplementation;
using QuickOutline.Scripts;
using UnityCommon.Modules;
using UnityCommon.Runtime.Extensions;
using UnityEngine;

namespace Fate.Modules
{
    public class AimAssist : Module
    {
        private Player m_Player;
        
        private Transform m_CurrentTarget;
        private Renderer m_CurrentTargetRenderer;
        
        private Conditional m_TargetDeathConditional;

        private Collider[] m_OverlappingEnemies = new Collider[25];

        private Material m_OutlineMaterial;
        
        // TODO: maybe do once on start
        public void Initialize()
        {
            m_Player = PlayerExtensions.GetPlayer();
            m_OutlineMaterial = Resources.Load<Material>("M_Outline");
        }

        public override void OnActiveSkill(ModuleRuntimeData runtimeData)
        {
            Initialize();

            var duration = runtimeData.GetDurationData();

            SetPlayerTarget();

            Conditional.For(duration).Do(SetPlayerTarget).OnComplete(() =>
            {
                if (m_CurrentTarget != null)
                {
                    m_CurrentTarget.GetComponent<Outline>().Destroy();
                }
                
                Player.AimAssistTarget = null;
            });
        }

        private void SetPlayerTarget()
        {
            FindTarget();
            Player.AimAssistTarget = m_CurrentTarget;
        }

        private void FindTarget()
        {
            var overlappingCont = FateExtensions.GetNearEnemies(m_Player.transform, ref m_OverlappingEnemies, 50f);

            if (m_CurrentTarget != null)
            {
                ResetOutline();
            }
            
            var newTarget =
                FateExtensions.GetClosestEnemy(overlappingCont, m_OverlappingEnemies, m_Player.transform.position);
            
            if(newTarget == m_CurrentTarget || newTarget == null)
                return;

            m_CurrentTarget = newTarget;
            m_CurrentTargetRenderer = m_CurrentTarget.GetComponent<Renderer>();
            SetOutline();
        }

        private void SetOutline()
        {
            
        }
        
        private void ResetOutline()
        {
            var matArray = m_CurrentTargetRenderer.materials;
           // matArray[i].
        }
    }
}