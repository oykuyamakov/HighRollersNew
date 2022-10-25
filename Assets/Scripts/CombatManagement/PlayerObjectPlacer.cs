using CharImplementations.PlayerImplementation;
using CharImplementations.PlayerImplementation.EventImplementations;
using CombatManagement.EventImplementations;
using Events;
using Fate.Modules;
using InputManagement;
using InputManagement.EventImplementations;
using Promises;
using Sirenix.OdinInspector;
using UnityCommon.Runtime.Extensions;
using UnityEngine;

namespace CombatManagement
{
    // TODO: IT'S ALMOST THE SAME AS PROJECTILE THROWER. DO SOMETHING ABOUT THAT.
    public class PlayerObjectPlacer : MonoBehaviour
    {
        public GameObject ObjectToBePlaced;
        public PlaceableBehaviour PlaceableBehaviour;

        public float MaxDistance;

        public Material InvalidPlacementMaterial;
        public Material ValidPlacementMaterial;

        private Transform m_PlayerTransform;

        private bool m_Active;

        private Promise<bool> m_PlaceObjectPromise;

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

            GEM.AddListener<ActivatePlayerObjectPlacerEvent>(OnActivatePlayerObjectPlacer);
        }

        private void OnActivatePlayerObjectPlacer(ActivatePlayerObjectPlacerEvent evt)
        {
            Setup(evt.MaxDistance, evt.ObjectToBePlaced);
            m_PlaceObjectPromise = evt.PlacedPromise;
        }

        private void OnPlayerInitialized(PlayerInitializedEvent evt)
        {
            m_PlayerTransform = Player.PlayerTransform;
        }

        [Button]
        protected void Setup(float maxDistance, GameObject prefab)
        {
            Time.timeScale *= 0.5f;

            MaxDistance = maxDistance;
            ObjectToBePlaced = prefab;

            PlaceableBehaviour = ObjectToBePlaced.AddComponent<PlaceableBehaviour>();

            GEM.AddListener<FireButtonPressedEvent>(OnPlaced);
            
            m_Active = true;
        }

        public void ClampObjectPosition()
        {
            var playerPos = m_PlayerTransform.position;
            var targetPos = InputManager.Instance.GetMousePos().WithY(playerPos.y);

            if (Vector3.Distance(playerPos, targetPos) > MaxDistance)
            {
                targetPos = playerPos + (targetPos - playerPos).normalized * MaxDistance;
            }

            ObjectToBePlaced.transform.position = targetPos;

            var rotation = Quaternion.LookRotation((targetPos - playerPos).normalized, Vector3.up);
            ObjectToBePlaced.transform.rotation = rotation;
            
            // TODO: don't do from here
            PlaceableBehaviour.AssignMaterial(PlaceableBehaviour.Placeable ? ValidPlacementMaterial : InvalidPlacementMaterial);
        }

        private void FixedUpdate()
        {
            if (m_Active)
                ClampObjectPosition();
        }

        [Button]
        public virtual void OnPlaced(FireButtonPressedEvent evt)
        {
            if (!m_Active)
                return;

            if(!PlaceableBehaviour.Placeable)
                return;
            
            m_Active = false;
            
            Time.timeScale *= 2f;
            m_PlaceObjectPromise.Complete(true);

            PlaceableBehaviour.Reset();
            
            GEM.RemoveListener<FireButtonPressedEvent>(OnPlaced);
        }
    }
}