using System;
using CharImplementations.PlayerImplementation.EventImplementations;
using Events;
using Fate.Modules;

namespace Fate.PassiveSkills
{
    [Serializable]
    public abstract class PassiveSkill
    {
        public ModuleParameter SkillParameter;

        public virtual void ApplyEffect(ModuleTier tier)
        {
        }
    }

    [Serializable]
    public class DefencePassiveSkill : PassiveSkill
    {
        public override void ApplyEffect(ModuleTier tier)
        {
            using (var evt = GetPlayerEvent.Get())
            {
                evt.SendGlobal();

                evt.Player.Health *= SkillParameter.TierParameterDataCollection[tier];
            }
        }
    }

    [Serializable]
    public class DamagePassiveSkill : PassiveSkill
    {
        public override void ApplyEffect(ModuleTier tier)
        {
            using (var evt = GetPlayerEvent.Get())
            {
                evt.SendGlobal();
            }
        }
    }

    [Serializable]
    public class SpeedPassiveSkill : PassiveSkill
    {
        public override void ApplyEffect(ModuleTier tier)
        {
            using (var evt = GetPlayerEvent.Get())
            {
                evt.SendGlobal();

                evt.Player.MoveSpeed *= SkillParameter.TierParameterDataCollection[tier];
            }
        }
    }
}