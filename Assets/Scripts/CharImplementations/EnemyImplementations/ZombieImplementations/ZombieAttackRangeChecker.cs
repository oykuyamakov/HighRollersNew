using CharImplementations.PlayerImplementation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CharImplementations.EnemyImplementations.ZombieImplementations
{
    [RequireComponent(typeof(Collider))]
    public class ZombieAttackRangeChecker : MonoBehaviour
    {
        [ShowInInspector]
        [ReadOnly]
        private Zombie m_Zombie;

        private Collider m_Collider;

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_Collider = GetComponent<Collider>();
            m_Collider.isTrigger = true;

            m_Zombie = GetComponentInParent<Zombie>();
        }
#endif
        
        private void Awake()
        {
            m_Collider = GetComponent<Collider>();
            m_Collider.isTrigger = true;

            m_Zombie = GetComponentInParent<Zombie>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                m_Zombie.SetAttackTarget(ZombieTarget.Player);
            }
            // TODO: DOOR
            // else if (other.TryGetComponent<Player>(out var player))
            // {
            //     
            // }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                m_Zombie.SetAttackTarget(ZombieTarget.None);
            }
        }
    }
}