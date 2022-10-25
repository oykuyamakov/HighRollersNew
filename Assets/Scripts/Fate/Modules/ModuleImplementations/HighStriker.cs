using CharImplementations.EnemyImplementations;
using CombatManagement.ProjectileManagement;
using Promises;
using Roro.Scripts.GameManagement;
using UnityEngine;

namespace Fate.Modules
{
    public class HighStriker : PlaceableModule
    {
        private HighStrikerBehaviour m_HighStrikerBehaviour;

        public override GameObject PlaceableObject => m_HighStrikerBehaviour.gameObject;

        private float m_StunDuration;
        private float m_StunRange;

        private float m_Health;
        private float m_HealthRegenarationPerSecond;

        private Promise<bool> m_Promise;

        private Collider[] m_EnemiesInRange = new Collider[25];

        public override void OnActiveSkill(ModuleRuntimeData runtimeData)
        {
            m_HighStrikerBehaviour = Object.Instantiate(m_PlaceableModuleData.PlaceableObjectPrefab)
                .GetComponent<HighStrikerBehaviour>();
            m_HighStrikerBehaviour.transform.localScale *= m_PlaceableModuleData.Scale.x;

            m_StunDuration = runtimeData.GetDurationData();
            m_StunRange = runtimeData.GetData(1);

            m_Health = runtimeData.GetData(2);
            m_HealthRegenarationPerSecond = runtimeData.GetData(3);
        }

        public override void OnPlaced()
        {
            m_Promise = m_HighStrikerBehaviour.Initialize(m_Health / m_HealthRegenarationPerSecond);
            m_Promise.OnResultT += ((b, b1) =>
            {
                OnStrike();
                m_Promise.Release();
            });
        }

        public void OnStrike()
        {
            StunEnemies();
            
            FateExtensions.GetParticle(ParticleType.HighStriker, m_HighStrikerBehaviour.transform);
            
            //TODO: not destroy
            Object.Destroy(m_HighStrikerBehaviour.gameObject, 1f);
        }

        public void StunEnemies()
        {
            int enemyCount =
                FateExtensions.GetNearEnemies(PlaceableObject.transform, ref m_EnemiesInRange, m_StunRange);

            for (var i = 0; i < enemyCount; i++)
            {
                if (m_EnemiesInRange[i].transform.TryGetComponent<Enemy>(out var enemy))
                {
                    enemy.GetStunned(m_StunDuration);
                }
                else if (m_EnemiesInRange[i].transform.TryGetComponent<Projectile>(out var projectile))
                {
                    if (projectile.HasHealth)
                    {
                        projectile.GetStunned(m_StunDuration);
                    }
                    else
                    {
                        projectile.DisableSelf();
                    }
                }
            }
        }
    }
}