using System;
using System.Collections.Generic;
using Fate.Modules.Data;
using Fate.PassiveSkills;
using Pooling;
using Roro.Scripts.Serialization;
using UnityEngine;

namespace Fate.Modules
{
    [Serializable]
    public class ModuleRuntimeData : IDisposable
    {
        public Module Module;

        public ModuleTier Tier;

        public ModuleData Data => Module.Data;

        public bool IsCursed => Module is CursedModule;

        public bool IsProjectile => Module is ProjectileModule;

        public bool IsPlaceable => Module is PlaceableModule;

        public int Slot = -1;

        public bool Equipped => Slot >= 0;

        public Dictionary<StatType, float> Stats = new Dictionary<StatType, float>();

        public void Initialize(object arg)
        {
            for (var i = 0; i < Data.PassiveSkills.Count; i++)
            {
                var skill = Data.PassiveSkills[i];
                Stats.Add(skill.StatType, skill.SkillParameter.GetParameter(Tier));
            }
        }

        public void OnActiveSkill(object arg)
        {
            Module.OnActiveSkill(this);
        }

        public void OnPassiveSkill(object arg)
        {
        }

        /// <summary>
        /// gets data indexed at 0.
        /// </summary>
        /// <returns></returns>
        public float GetDurationData()
        {
            return GetData(0);
        }

        public float GetData(int dataIndex)
        {
            return Data.Parameters[dataIndex].GetParameter(Tier);
        }

        public float GetPassiveStatValue(StatType type)
        {
            return Stats.ContainsKey(type) ? Stats[type] : 0;
        }

        // TODO: tuple makes it hard-coded.
        public (int, int, int) GetModuleStats()
        {
            var vitality = 0;
            var speed = 0;
            var damage = 0;

            foreach (var stat in Stats)
            {
                switch (stat.Key)
                {
                    case StatType.Vitality:
                        vitality += (int)stat.Value;
                        break;
                    case StatType.Speed:
                        speed += (int)stat.Value;
                        break;
                    case StatType.Damage:
                        damage += (int)stat.Value;
                        break;
                }
            }

            return (vitality, speed, damage);
        }

        public List<int> GetModuleStatsList()
        {
            return new List<int>() { GetModuleStats().Item1, GetModuleStats().Item2, GetModuleStats().Item3 };
        }

        // TODO: i don't know if i need this shit
        public void Dispose()
        {
        }

        public void Save(int saveIndex)
        {
            SerializationWizard m_SerializationWizard = SerializationWizard.Default;

            m_SerializationWizard.SetInt($"module_{saveIndex}", Data.Index); // set module Id
            m_SerializationWizard.SetInt($"module_{saveIndex}_tier", (int)Tier);
            m_SerializationWizard.SetInt($"module_{saveIndex}_slot", Slot);

            m_SerializationWizard.Push();
        }

        public void Load(int loadIndex)
        {
            SerializationWizard m_SerializationWizard = SerializationWizard.Default;

            Module = ModuleManager.Modules[m_SerializationWizard.GetInt($"module_{loadIndex}")];
            Tier = (ModuleTier)m_SerializationWizard.GetInt($"module_{loadIndex}_tier");
            Slot = m_SerializationWizard.GetInt($"module_{loadIndex}_slot");

            Initialize(null);
        }
    }
}