using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace SideMissionManagement
{
    [Serializable]
    public enum SideMissionSequenceItemType
    {
        Wave,
        Dialogue,
        Event,
        Boss,
        Wait
    }

    public enum SideMissionStateType
    {
    }

    [Serializable]
    public class SideMissionSequenceItem
    {
        public SideMissionSequenceItemType Type;

        [ShowIf("Type", SideMissionSequenceItemType.Wave)]
        public WaveData WaveData;

        // [ShowIf("Type", SideMissionSequenceItemType.Dialogue)]
        // public dialogue DialogueData;
    }

    [CreateAssetMenu(menuName = "Side Mission/Side Mission Data")]
    public class SideMissionData : ScriptableObject
    {
        public List<SideMissionSequenceItem> Sequence = new List<SideMissionSequenceItem>();
        
        [ListDrawerSettings(Expanded = false)]
        public List<WaveData> Waves = new List<WaveData>();

#if UNITY_EDITOR
        private void OnValidate()
        {
            Waves.Clear();

            var count = 0;
            foreach (var sequenceItem in Sequence)
            {
                if (sequenceItem.Type == SideMissionSequenceItemType.Wave)
                {
                    sequenceItem.WaveData.WaveId = count;
                    Waves.Add(sequenceItem.WaveData);
                    count++;
                }
            }
            
            EditorUtility.SetDirty(this);
        }
#endif
    }
}