using CharImplementations;
using Roro.Scripts.GameManagement;
using UnityCommon.Modules;
using UnityCommon.Runtime.Extensions;
using UnityEngine;

namespace Fate.Modules
{
    public class Disabled : CursedModule
    {
        public override void OnActiveSkill(ModuleRuntimeData runtimeData)
        {
            var player = PlayerExtensions.GetPlayer();

            var particle = FateExtensions.GetParticle(ParticleType.Disabled, player.transform, false);
            
            particle.SelfTransform().localPosition = Vector3.zero.WithY(-3.5f);

            player.GetStunned(runtimeData.GetDurationData());

            Conditional.Wait(runtimeData.GetDurationData())
                .Do(() => { particle.Disable(); });
        }
    }
}