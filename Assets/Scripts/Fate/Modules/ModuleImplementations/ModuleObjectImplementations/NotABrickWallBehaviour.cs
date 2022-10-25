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

        private Conditional m_TimedDestroyConditional;

        public void Setup(float health, float duration)
        {
            CurrentHealth = health;
            m_Duration = duration;
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnContact(collision.transform);
        }

        private void OnTriggerEnter(Collider other)
        {
            OnContact(other.transform);
        }

        public override void OnContact(Transform contactT)
        {
            if (contactT.TryGetComponent<Projectile>(out var projectile))
            {
                if (projectile.TargetType == CharType.Enemy)
                    return;
                
                if(projectile.HasHealth)
                    return;
                
                projectile.DisableSelf();
                
                CurrentHealth -= projectile.Damage;
                if (CurrentHealth <= 0)
                {
                    m_TimedDestroyConditional.Cancel();
                    // TODO: DON'T DESTROY
                    Destroy(gameObject);
                }
            }
        }

        // TODO: DON'T DESTROY
        public void StartTimer()
        {
            m_TimedDestroyConditional = Conditional.Wait(m_Duration)
                .Do(() => Destroy(gameObject));
        }
    }
}