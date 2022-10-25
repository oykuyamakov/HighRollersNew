using Sirenix.OdinInspector;
using UnityEngine;

namespace Fate
{
    [CreateAssetMenu(menuName = "Fate/FateSettings")]
    public class FateSettings : ScriptableObject
    {
        private static FateSettings s_FateSettings;

        private static FateSettings fateSettings
        {
            get
            {
                if (!s_FateSettings)
                {
                    s_FateSettings = Resources.Load<FateSettings>($"Settings/FateSettings");

                    if (!s_FateSettings)
                    {
#if UNITY_EDITOR
                        Debug.Log("Creating Fate Settings");
                        s_FateSettings = CreateInstance<FateSettings>();
#else
 				//		throw new Exception("Global settings could not be loaded");
#endif
                    }
                }

                return s_FateSettings;
            }
        }

        public static FateSettings Get()
        {
            return fateSettings;
        }

        [BoxGroup("Combat Values")]
        public int DamageEnergyFillAmount = 10;
        [BoxGroup("Combat Values")]
        public int AttackEnergyFillAmount = 10;
        
        [BoxGroup("Auto Filling Values")]
        public int IncrementalFillingAmount = 10;
        [BoxGroup("Auto Filling Values")]
        public int IncrementalFillingInterval = 10;
        
        public int MaxEnergy = 100;
    }
}
