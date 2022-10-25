using CharImplementations.PlayerImplementation;
using CharImplementations.PlayerImplementation.EventImplementations;
using CombatManagement.EventImplementations;
using CombatManagement.ProjectileManagement;
using Events;
using InputManagement.EventImplementations;
using Promises;
using Sirenix.OdinInspector;
using UnityEngine;
using InputManager = InputManagement.InputManager;

namespace CombatManagement
{
    public class PlayerProjectileThrower : MonoBehaviour
    {
        public GameObject ProjectileIndicator;

        public float MaxThrowingDistance;

        public ProjectileType ProjectileType;

        private Transform m_PlayerTransform;

        private bool m_Active;

        // private Promise<Projectile> m_ProjectilePromise;

        private Promise<Transform> m_ProjectilePromise;

        protected void OnEnable()
        {
            if (Player.PlayerTransform == null)
            {
                GEM.AddListener<PlayerInitializedEvent>(OnPlayerInitialized);
            }
            else
            {
                m_PlayerTransform = Player.PlayerTransform;
            }

            GEM.AddListener<ActivatePlayerProjectileThrowerEvent>(OnActivatePlayerProjectileThrower);
        }

        private void OnActivatePlayerProjectileThrower(ActivatePlayerProjectileThrowerEvent evt)
        {
            Setup(evt.MaxDistance, evt.ProjectileType);
            m_ProjectilePromise = evt.ProjectilePromise;
        }

        private void OnPlayerInitialized(PlayerInitializedEvent evt)
        {
            m_PlayerTransform = Player.PlayerTransform;
        }

        [Button]
        protected void Setup(float maxDistance, ProjectileType projectileType)
        {
            m_Active = true;
            MaxThrowingDistance = maxDistance;
            ProjectileType = projectileType;

            ProjectileIndicator.SetActive(true);

            GEM.AddListener<FireButtonPressedEvent>(OnThrow);
        }

        public void ClampProjectilePosition()
        {
            var targetPos = InputManager.Instance.GetMousePos();
            var playerPos = m_PlayerTransform.position;

            if (Vector3.Distance(playerPos, targetPos) > MaxThrowingDistance)
            {
                targetPos = playerPos + (targetPos - playerPos).normalized * MaxThrowingDistance;
            }

            ProjectileIndicator.transform.position = targetPos;
            ProjectileIndicator.transform.LookAt(playerPos);
        }

        private void Update()
        {
            if (m_Active)
                ClampProjectilePosition();
        }

        [Button]
        public virtual void OnThrow(FireButtonPressedEvent evt)
        {
            m_ProjectilePromise.Complete(ProjectileIndicator.transform);

            m_Active = false;
            ProjectileIndicator.SetActive(false);

            GEM.RemoveListener<FireButtonPressedEvent>(OnThrow);
        }
    }
}