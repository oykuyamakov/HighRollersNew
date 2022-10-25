using System.Collections.Generic;
using System.Linq;
using CharImplementations;
using CharImplementations.PlayerImplementation;
using CombatManagement.EventImplementations;
using CombatManagement.ProjectileManagement;
using CombatManagement.ProjectileManagement.Implementations;
using Events;
using Fate.Modules;
using Pooling.EventImplementations;
using Roro.Scripts.Utility;
using Unity.VisualScripting;
using UnityCommon.Runtime.Extensions;
using UnityEngine;

namespace CombatManagement
{
    [DefaultExecutionOrder(ExecOrder.FireManager)]
    public class FireManager : MonoBehaviour
    {
        [SerializeField] 
        private List<Projectile> m_BulletDatas;
        private Transform m_PoolParent => this.transform;

        private Dictionary<ProjectileType, Pooling.ObjectPool<Projectile>>
            m_ProjectilePoolsByTypes = new();

        private Dictionary<ProjectileType, Projectile> m_ProjectilePrefabsByTypes =>
            m_BulletDatas.ToDictionary(key => key.ProjectileType, k => k);
        
        private int PoolSize;

        private void Awake()
        {
            GEM.AddListener<GetProjectileEvent>(OnGetProjectileEvent);
            GEM.AddListener<PoolReleaseEvent<Projectile>>(DequeueProjectile);
        }

        private void OnDestroy()
        {
            GEM.RemoveListener<GetProjectileEvent>(OnGetProjectileEvent);
            GEM.RemoveListener<PoolReleaseEvent<Projectile>>(DequeueProjectile);
        }

        private Projectile GetProjectile(ProjectileType type)
        {
            if (!m_ProjectilePoolsByTypes.ContainsKey(type))
            {
                m_ProjectilePoolsByTypes.Add(type, new Pooling.ObjectPool<Projectile>(100));
            }

            return m_ProjectilePoolsByTypes[type]
                .GetPoolable(m_ProjectilePrefabsByTypes[type].gameObject, m_PoolParent).Get();
        }
        
        public void OnGetProjectileEvent(GetProjectileEvent evt)
        {
            var pr = GetProjectile(evt.ProjectileType);
            pr.GameObject().SetActive(true);
            evt.Projectile = pr;
        }

        //do we need creator?
        public static Vector3 FireRay(Vector3 origin, Vector3 targetPos, float magnitude, float damage, CharType targetType, LayerMask layerMask, Transform creator)
        {
            var dir = (targetPos.WithY(Player.GlobalProjectileY) - origin.WithY(Player.GlobalProjectileY)).normalized;

            if (Physics.Raycast(origin.WithY(Player.GlobalProjectileY), dir, out RaycastHit hit, magnitude, layerMask))
            {
                if (hit.transform.TryGetComponent<Character>(out var chara))
                {
                    chara.OnImpact(targetType, -damage);
                }
                //to do should take this somewhere else?
                if (hit.transform.TryGetComponent<DuganDice>(out var mPro))
                {
                    mPro.OnHit(creator);
                }
                if (hit.transform.TryGetComponent<Missile>(out var missile))
                {
                    missile.OnContact(creator);
                }

                if (hit.transform.TryGetComponent<PlaceableModuleBase>(out var placeableModuleBase))
                {
                    placeableModuleBase.OnContact(damage);
                }

                return hit.point;
            }

            Vector3 rayDir = dir.WithY(Player.GlobalProjectileY) * magnitude;
            Debug.DrawRay(origin, rayDir, new Color(0.8f,0f,0,0.9f));
            
            return (origin + rayDir);
        }

        public void DequeueProjectile(PoolReleaseEvent<Projectile> evt)
        {
            var pr = evt.Poolable.Return(m_PoolParent);
            pr.SelfTransform().SetParent(null);

            m_ProjectilePoolsByTypes[pr.ProjectileType].ReleasePoolable(evt.Poolable);
        }
    }
}