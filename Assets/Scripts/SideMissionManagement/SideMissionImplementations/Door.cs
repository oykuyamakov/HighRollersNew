using UnityEngine;

// TODO: definitely refactor
namespace SideMissionManagement.SideMissionImplementations
{
    public class Door : MonoBehaviour
    {
        public SideMissionController SideMissionController;

        public int DoorHealth;

        public void TakeDamage(int damage)
        {
            DoorHealth -= damage;
            if (DoorHealth <= 0)
            {
                SideMissionController.OnSideMissionFailed();
            }
        }

        // private void OnCollisionEnter(Collision other)
        // {
        //     if (other.gameObject.TryGetComponent<IDamager>(out var damager))
        //     {
        //         TakeDamage((int)damager.Damage);
        //     }
        // }
        //
        // private void OnTriggerEnter(Collider other)
        // {
        //     if (other.gameObject.TryGetComponent<IDamager>(out var damager))
        //     {
        //         TakeDamage((int)damager.Damage);
        //     }
        // }
    }
}