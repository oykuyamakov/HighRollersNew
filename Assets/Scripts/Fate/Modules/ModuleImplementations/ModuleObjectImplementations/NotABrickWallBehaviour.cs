using CharImplementations;
using CombatManagement.ProjectileManagement;
using UnityCommon.Modules;
using UnityEngine;

namespace Fate.Modules
{
    // TODO: PLACEABLE OBJECT BASE CLASS
    public class NotABrickWallBehaviour : PlaceableModuleBase
    {
        public float CurrentHealth;

        private float m_Duration;

        public void Setup(float health, float duration)
        {
            CurrentHealth = health;
            m_Duration = duration;
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnContact(collision.transform);
        }

        public override void OnContact(Transform contactT)
        {
            if (contactT.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.DisableSelf();

                if (projectile.TargetType == CharType.Enemy)
                    return;

                CurrentHealth -= projectile.Damage;
                if (CurrentHealth <= 0)
                {
                    // TODO: DON'T DESTROY
                    Destroy(gameObject);
                }
            }
        }

        // TODO: DON'T DESTROY
        public void StartTimer()
        {
            Conditional.Wait(m_Duration)
                .Do(() => Destroy(gameObject));
        }
    }
}