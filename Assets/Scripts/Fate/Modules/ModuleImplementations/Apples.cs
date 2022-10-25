using CharImplementations;
using CharImplementations.PlayerImplementation;
using Roro.Scripts.GameManagement;
using UnityCommon.Modules;
using Utility.Extensions;

namespace Fate.Modules
{
    public class Apples : Module
    {
        private Conditional m_HealingCond;
        private Player m_Player;

        public override void OnActiveSkill(ModuleRuntimeData runtimeData)
        {
            m_Player = PlayerExtensions.GetPlayer();

            float healingTime = runtimeData.GetDurationData();
            float healingAmount = runtimeData.GetData(1);

            m_HealingCond = Conditional.Repeat(1f, (int)healingTime,
                () => OnHealingAction(healingAmount.SafeDivision(healingTime)));
        }

        private void OnHealingAction(float healingAmount)
        {
            m_Player.ChangeHealth(healingAmount);

            FateExtensions.GetParticle(ParticleType.HealthUp, m_Player.transform);
        }
    }
}