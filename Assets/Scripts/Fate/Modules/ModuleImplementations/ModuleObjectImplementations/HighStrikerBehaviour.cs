using CharImplementations;
using CombatManagement.ProjectileManagement;
using DG.Tweening;
using Promises;
using UnityCommon.Modules;
using UnityCommon.Runtime.Extensions;
using UnityEngine;

namespace Fate.Modules
{
    public class HighStrikerBehaviour : PlaceableModuleBase
    {
        public GameObject BarPivot;

        private Tween m_CurrentTween;

        private float m_HealthFillingTime;

        private Promise<GameObject> m_Promise;

        private Collider m_Collider;

        private bool m_Initialized;

        private void Awake()
        {
            m_Collider = GetComponent<Collider>();
        }

        public Promise<GameObject> Initialize(float healthFillingTime)
        {
            m_HealthFillingTime = healthFillingTime;

            m_Promise = Promise<GameObject>.Create();

            m_Initialized = true;

            return m_Promise;
        }

        private void OnTriggerEnter(Collider other)
        {
            OnContact(other.transform);
        }

        public override void OnContact(Transform contactT)
        {
            if (!m_Initialized)
                return;

            if (contactT.TryGetComponent<Projectile>(out var projectile) &&
                projectile.TargetType == CharType.Enemy)
            {
                var damage = projectile.Damage;
                OnGetDamage(damage);
            }
        }

        public override void OnContact(float damage)
        {
            if (!m_Initialized)
                return;

            OnGetDamage(damage);
        }

        private void OnGetDamage(float damage)
        {
            m_CurrentTween.Kill();

            BarPivot.transform.AddScaleYClamped(damage, 1);

            if (BarPivot.transform.localScale.y >= 1)
            {
                m_Collider.enabled = false;
                m_Promise.Complete(gameObject);

                Conditional.WaitFrames(1)
                    .Do(() => m_Promise.Release());

                return;
            }

            m_CurrentTween = BarPivot.transform.DOScaleY(0, m_HealthFillingTime);
        }
    }
}