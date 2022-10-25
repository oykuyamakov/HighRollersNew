using System;
using Fate.Modules.Data;
using Roro.Scripts.Serialization;

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

        public void Initialize(object arg)
        {
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
        }
    }
}