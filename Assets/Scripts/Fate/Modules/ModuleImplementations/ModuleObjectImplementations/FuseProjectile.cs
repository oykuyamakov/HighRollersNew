using CharImplementations;
using CombatManagement.ProjectileManagement;
using UnityCommon.Runtime.Extensions;
using UnityEngine;

namespace Fate.Modules
{
    // TODO: NOT CURRENTLY USED
    public class FuseProjectile : Projectile
    {
        public override void Initialize(Vector3 origin, Vector3 direction,
            float damage, CharType targetType, LayerMask layersToCollide, string layer)
        {
            ProjectileDamage = damage;
            TargetType = targetType;
            SelfLayer = layer;
            TargetLayers = layersToCollide;

            m_Enabled = true;

            SelfTransform().position = origin.WithY(m_YOffset);
            SelfTransform().forward = direction;
        }

        public override void OnContact(Transform t)
        {
            var fuseDir = SelfTransform().forward;
            var knockBackDir = new Vector3(fuseDir.z, fuseDir.y, -fuseDir.x); // perpendicular to fuseDir
            
            if (t.TryGetComponent<Projectile>(out var pro))
            {
                if (pro.HasHealth)
                {
                    pro.SelfRigid().AddForce(knockBackDir * ProjectileDamage);
                    return;
                }
                
                pro.DisableSelf();
                return;
            }

            if (t.TryGetComponent<Character>(out var chara))
            {
                if (chara.GetCharType() == TargetType)
                {
                    chara.GetKnocked(0.5f, knockBackDir * ProjectileDamage);
                }
            }
        }
    }
}