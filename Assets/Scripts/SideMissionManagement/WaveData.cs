using System;
using System.Collections.Generic;
using CharImplementations.EnemyImplementations;
using Sirenix.OdinInspector;

namespace SideMissionManagement
{
    [Serializable]
    public enum WaveType
    {
        MainWave,
        MidWave
    }
    
    [Serializable]
    public struct EnemyCountPair
    {
        public Enemy Enemy;
        public int Count;
    }
    
    [Serializable]
    public class WaveData
    {
        [ReadOnly]
        public int WaveId;

        public WaveType Type;
        
        public float WaveDuration;
        
        [ShowIf("@Type == WaveType.MidWave")]
        public float EnemySpawnInterval;
        
        [TableList]
        public List<EnemyCountPair> WaveEnemies = new List<EnemyCountPair>();
    }
}