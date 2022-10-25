using System;
using CharImplementations.PlayerImplementation.EventImplementations;
using Events;
using Fate.Modules;

namespace Fate.PassiveSkills
{
    public enum StatType
    {
        Vitality = 0,
        Speed = 1,
        Damage = 2,
    }
    
    [Serializable]
    public abstract class PassiveSkill
    {
        public ModuleParameter SkillParameter;
        public abstract StatType StatType { get; }

        public virtual void ApplyEffect(ModuleTier tier)
        {
        }
    }

    [Serializable]
    public class DefencePassiveSkill : PassiveSkill
    {
        public override StatType StatType => StatType.Vitality;
        
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
        public override StatType StatType => StatType.Damage;

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
        public override StatType StatType => StatType.Speed;

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