using DG.Tweening;
using UnityCommon.Runtime.Extensions;
using UnityEngine;

namespace CharImplementations.EnemyImplementations.ZombieImplementations
{
    public class Tombstone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("ProjectilePlayer"))
            {
                BreakDown();
                return;
            }

            // if (other.transform.TryGetComponent<Zombie>(out Zombie zombie))
            // {
            //     Destroy(zombie.gameObject);
            // }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("ProjectilePlayer"))
            {
                BreakDown();
                return;
            }

            // if (collision.transform.TryGetComponent<Zombie>(out Zombie zombie))
            // {
            //     Destroy(zombie.gameObject);
            // }
        }

        private void BreakDown()
        {
            // TODO: breakdown
            transform.DOLocalRotate(Vector3.zero.WithX(-90), 0.35f).SetEase(Ease.OutBack);
        }
    }
}