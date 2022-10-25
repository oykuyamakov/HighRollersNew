using System.Collections.Generic;
using CharImplementations.EnemyImplementations;
using Events;
using Promises;
using SideMissionManagement.EventImplementations;
using Sirenix.OdinInspector;
using UnityCommon.Runtime.Extensions;
using UnityCommon.Runtime.Utility;
using UnityEngine;

namespace SideMissionManagement
{
    public class WaveController : MonoBehaviour
    {
        [BoxGroup("Wave Data")]
        public WaveData WaveData;

        [ShowInInspector] public int TotalEnemyCount => WaveData.WaveEnemies.Count;

        [ShowInInspector] public float WaveDuration => WaveData.WaveDuration;

        public Promise<bool> WavePromise;

        [ReadOnly]
        public int EnemiesTakenDown;

        [ReadOnly]
        public float RemainingTime;

        public TimedAction EnemySpawnTimer;
        public TimedAction WaveAction;

        private List<Collider> m_SpawnAreas;

        private List<Enemy> m_WaveEnemies = new List<Enemy>();

        private void Awake()
        {
            WavePromise = Promise<bool>.Create();
        }

        public Promise<bool> Initialize(WaveData data, List<Collider> spawnAreas)
        {
            m_SpawnAreas = spawnAreas;
            WaveData = data;

            EnemiesTakenDown = 0;
            RemainingTime = WaveDuration;

            InitializeEnemies();

            WaveAction = new TimedAction(WaveUpdate, 0f, 1f);

            return WavePromise;
        }

        private void InitializeEnemies()
        {
            if (WaveData.Type == WaveType.MidWave)
            {
                EnemySpawnTimer = new TimedAction(() => InitializeEnemy(WaveData.WaveEnemies[0].Enemy), 0f,
                    WaveData.EnemySpawnInterval);

                return;
            }

            for (var i = 0; i < WaveData.WaveEnemies.Count; i++)
            {
                var enemy = WaveData.WaveEnemies[i].Enemy;
                var count = WaveData.WaveEnemies[i].Count;

                for (int j = 0; j < count; j++)
                {
                    InitializeEnemy(enemy);
                }
            }
        }

        private void InitializeEnemy(Enemy enemy)
        {
            if (WaveData.Type == WaveType.MidWave && m_WaveEnemies.Count >= WaveData.WaveEnemies[0].Count)
                return;

            var newEnemyGo = Instantiate(enemy);
            newEnemyGo.transform.position = GetRandomSpawnLocation();

            var newEnemy = newEnemyGo.GetComponent<Enemy>();

            newEnemy.Setup();

            m_WaveEnemies.Add(newEnemy);
        }

        public Vector3 GetRandomSpawnLocation()
        {
            var randomSpawnAreaEnumerator = m_SpawnAreas.RandomTake(1).GetEnumerator();
            randomSpawnAreaEnumerator.MoveNext();

            var randomSpawnArea = randomSpawnAreaEnumerator.Current;

            randomSpawnAreaEnumerator.Dispose();

            return randomSpawnArea.GetRandomPointInBounds();
        }

        private void Update()
        {
            WaveAction?.Update(Time.deltaTime);
            EnemySpawnTimer?.Update(Time.deltaTime);
        }

        private void WaveUpdate()
        {
            RemainingTime--;

            if (RemainingTime <= 0)
            {
                FinishWave();
            }
        }

        [Button]
        public void FinishWave()
        {
            WaveAction = null;
            EnemySpawnTimer = null;
            m_WaveEnemies.Clear();

            using var evt = WaveCompletedEvent.Get(WaveData.WaveId).SendGlobal();
        }

        [Button]
        public void ResetWave()
        {
            EnemiesTakenDown = 0;
            RemainingTime = WaveDuration;

            for (var i = 0; i < m_WaveEnemies.Count; i++)
            {
                m_WaveEnemies[i].gameObject.Destroy();
            }

            InitializeEnemies();

            WaveAction = new TimedAction(WaveUpdate, 0f, 1f);
            EnemySpawnTimer = null;
        }
    }
}