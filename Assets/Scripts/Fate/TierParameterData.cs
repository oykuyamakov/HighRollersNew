using System;
using Fate.Modules;
using Sirenix.OdinInspector;
using Utility;

namespace Fate
{
    public enum ModuleParameterType
    {
        Constant,
        Tiered
    }

    [Serializable]
    public class ModuleParameter
    {
        [FoldoutGroup("$ParameterName")]
        public string ParameterName;

        [EnumToggleButtons]
        public ModuleParameterType ParameterType;

        [ShowIf("ParameterType", ModuleParameterType.Constant)]
        public float Parameter;

        [ShowIf("ParameterType", ModuleParameterType.Tiered)]
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine, KeyLabel = "Tier",
            ValueLabel = "Parameter")]
        public TierParameterDataCollection TierParameterDataCollection = new TierParameterDataCollection()
        {
            { ModuleTier.Common, 0 },
            { ModuleTier.Uncommon, 0 },
            { ModuleTier.Rare, 0 },
            { ModuleTier.Epic, 0 },
            { ModuleTier.Legendary, 0 },
        };

        public float GetParameter(ModuleTier tier)
        {
            if (ParameterType == ModuleParameterType.Constant)
                return Parameter;

            return TierParameterDataCollection[tier];
        }
    }

    [Serializable]
    public class TierParameterDataCollection : UnitySerializedDictionary<ModuleTier, float>
    {
    }
}