using System;
using CharImplementations;
using CombatManagement.EventImplementations;
using Events;
using Sirenix.OdinInspector;
using UnityCommon.Runtime.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CombatManagement.WeaponImplementation
{
    public class WeaponBehaviour : MonoBehaviour, IDamager
    {
        [FormerlySerializedAs("weaponObject")][SerializeField]
        private WeaponObject m_WeaponObject;

        public WeaponData WeaponData => m_WeaponObject.WeaponData;
        public float Damage => WeaponData.Damage;
        private WeaponType m_WeaponType => WeaponData.GetWeaponType();

        [SerializeField]
        private Transform m_OriginPos;

        [SerializeField] private ParticleSystem m_Muzzle;
        
        [SerializeField] [ShowIf("@m_WeaponType == WeaponType.Beam")]
        private LineRenderer m_LineRenderer;

        [SerializeField] [ShowIf("@m_WeaponType == WeaponType.Beam")]
        private ParticleSystem m_EndParticle;

        public void Attack(Vector3 originPos, Vector3 targetPos, CharType charType,bool aiming)
        {
            var layerMask = 1 << LayerMask.NameToLayer("ProjectileEnemy") |
                            1 << LayerMask.NameToLayer("Enemy") |
                            1 << LayerMask.NameToLayer("Obstacle") |
                            1 << LayerMask.NameToLayer("Level");
            
            if (WeaponData.GetWeaponType() == WeaponType.Automatic || WeaponData.GetWeaponType() == WeaponType.SingleShot)
            {
                var projectile = GetProjectileEvent.Get(WeaponData.ProjectileType).SendGlobal().Projectile;
                

                var pos = targetPos;
                
                if (WeaponData.Spread && !aiming)
                {
                    float rand = Random.Range(0, 1f);

                    if (rand < WeaponData.NarrowSpreadProbability && rand > WeaponData.WiderSpreadProbability)
                    {
                        var angle = Random.Range(WeaponData.NarrowSpreadAngle,WeaponData.WiderSpreadAngle);
                        angle *= Random.Range(0f, 1f) > 0.5f ? 1 : -1;
                        pos = RotatePointAroundPivot2(targetPos.WithY(0), originPos.WithY(0), Vector3.up * angle);
                    }
                    else if (rand < WeaponData.WiderSpreadProbability)
                    {
                        var narrowAngle = aiming ? WeaponData.NarrowSpreadAngle / 2 : WeaponData.NarrowSpreadAngle;
                        var angle = Random.Range(0,narrowAngle);
                        angle *= Random.Range(0f, 1f) > 0.5f ? 1 : -1;
                        pos = RotatePointAroundPivot2(targetPos.WithY(0), originPos.WithY(0), Vector3.up * angle);
                    }
                }
              
                var tp = m_OriginPos != null ? m_OriginPos.position.WithY(6) : originPos.WithY(6);
                projectile.Initialize(tp, pos, WeaponData.Damage, charType, layerMask, "ProjectilePlayer");
                projectile.Mover.MoveToPos(pos, WeaponData.ProjectileSpeed, WeaponData.Range);

            }
            else
            {

                var hitPos = FireManager.FireRay(originPos,
                    targetPos, WeaponData.Range, WeaponData.Damage, charType, layerMask, transform);
                
                m_LineRenderer.enabled = true;

                m_LineRenderer.positionCount = 2;
                
                m_LineRenderer.transform.rotation = Quaternion.identity;
                
                var posX = hitPos.x - transform.position.x;
                
                if (m_EndParticle != null)
                {
                    m_EndParticle.gameObject.SetActive(true);
                    m_EndParticle.Play();
                    m_EndParticle.transform.position = hitPos;
                }
                

                m_LineRenderer.SetPositions(new[]{ transform.position.WithY(6), hitPos.WithY(6) });
            }

            if (m_Muzzle != null)
            {
                m_Muzzle.Play();
                m_Muzzle.gameObject.SetActive(true);
            }

        }
        
        private Vector3 RotatePointAroundPivot2(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            var dir = point - pivot; // get point direction relative to pivot
            dir = Quaternion.Euler(angles) * dir; // rotate it
            point = dir + pivot; // calculate rotated point
            return point; // return it
        }

        public void Disable()
        {

            if (WeaponData.GetWeaponType() == WeaponType.Beam)
            {
                m_LineRenderer.enabled = false;
                m_EndParticle.gameObject.SetActive(false);
                m_Muzzle.gameObject.SetActive(false);


            }
        }
    }
}