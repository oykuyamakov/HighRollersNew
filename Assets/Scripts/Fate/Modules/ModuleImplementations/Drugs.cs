using CharImplementations;
using Misc;
using Rewired;
using Roro.Scripts.GameManagement;
using UnityCommon.Modules;

namespace Fate.Modules
{
    public class Drugs : Module
    {
        private float m_OriginalSpeed;

        public override void OnActiveSkill(ModuleRuntimeData runtimeData)
        {
            var player = PlayerExtensions.GetPlayer();

            m_OriginalSpeed = player.MoveSpeed;
            player.MoveSpeed += runtimeData.GetData(1) * m_OriginalSpeed;

            var particle = FateExtensions.GetParticle(ParticleType.SpeedUp, player.transform, false);

            Conditional.Wait(runtimeData.GetDurationData())
                .Do(() =>
                {
                    player.MoveSpeed -= runtimeData.GetData(1) * m_OriginalSpeed;
                    particle.Disable();
                });
        }
    }
}