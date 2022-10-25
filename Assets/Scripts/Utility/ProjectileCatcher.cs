using System;
using CombatManagement.ProjectileManagement;
using UnityEngine;

namespace Utility
{
    public class ProjectileCatcher : MonoBehaviour
    {
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<Projectile>(out var pro))
            {
                pro.DisableSelf();
            }
        }
    }
}
