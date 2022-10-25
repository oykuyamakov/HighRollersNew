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
        private Transform m_CurrentTarget;
        private Player m_Player;

        private Conditional m_TargetDeathConditional;

        private Collider[] m_OverlappingEnemies = new Collider[25];

        // TODO: maybe do once on start
        public void Initialize()
        {
            m_Player = PlayerExtensions.GetPlayer();
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
                // TODO: outline stuff
            }
            
            var newTarget =
                FateExtensions.GetClosestEnemy(overlappingCont, m_OverlappingEnemies, m_Player.transform.position);
            
            if(newTarget == m_CurrentTarget || newTarget == null)
                return;

            m_CurrentTarget = newTarget;
            Debug.Log("Current Target " + newTarget.name);
            //TODO: outline stuff
        }
    }
}