using CharImplementations;
using Roro.Scripts.GameManagement;
using UnityCommon.Modules;

namespace Fate.Modules
{
    public class Confusion : CursedModule
    {
        public override void OnActiveSkill(ModuleRuntimeData runtimeData)
        {
            var player = PlayerExtensions.GetPlayer();
            player.InvertedControls = true;

            var particle = FateExtensions.GetParticle(ParticleType.Cursed, player.transform, false);

            Conditional.Wait(runtimeData.GetDurationData())
                .Do(() =>
                {
                    player.InvertedControls = false;
                    particle.Disable();
                });
        }
    }
}