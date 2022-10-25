using CharImplementations;
using CombatManagement.EventImplementations;
using CombatManagement.ProjectileManagement;
using Events;
using UnityCommon.Runtime.Extensions;
using UnityEngine;

namespace Fate.Modules
{
    public class FloydBehaviour : PlaceableModuleBase
    {
        public int NumberOfBullets;

        private Collider m_Collider;

        private void OnEnable()
        {
            m_Collider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            OnContact(other.transform);
        }

        public override void OnContact(Transform contactT)
        {
            m_Collider.enabled = false;

            if (contactT.TryGetComponent<Projectile>(out var projectile))
            {
                if (projectile.TargetType == CharType.Enemy)
                {
                    var vector1 = Quaternion.AngleAxis(120, Vector3.up) *
                                  -projectile.SelfTransform().forward;

                    var anglePerBullet = 120 / NumberOfBullets;

                    for (int i = 0; i < NumberOfBullets; i++)
                    {
                        var moveDir = Quaternion.AngleAxis(anglePerBullet * i, Vector3.down) *
                                      vector1;

                        using var evt = GetProjectileEvent.Get(projectile.ProjectileType);
                        {
                            evt.SendGlobal();

                            var newProjectile = evt.Projectile;

                            newProjectile.Initialize(transform.position, projectile.transform.forward,
                                projectile.Damage, CharType.Enemy, Physics.AllLayers,
                                "ProjectilePlayer");

                            var projectileInfo = new ProjectileInfo();

                            projectileInfo.MovementType = ProjectileInfo.ProjectileMovementType.LinearMove;
                            projectileInfo.Damage = projectile.Damage;

                            newProjectile.Mover.MoveToPos((moveDir * 10).WithY(projectile.SelfTransform().position.y),
                                10);
                        }
                    }
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
        }
    }
}