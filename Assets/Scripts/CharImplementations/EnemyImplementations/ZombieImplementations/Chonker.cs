using CharImplementations.PlayerImplementation;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityCommon.Runtime.Utility;
using UnityEngine;
using Utility.Extensions;

namespace CharImplementations.EnemyImplementations.ZombieImplementations
{
    public enum ChonkerAttackType
    {
        Throw,
        Charge
    }

    public class Chonker : Zombie
    {
        [SerializeField]
        private Transform m_GrabbedPosition;

        private Collider[] m_NearByColliders = new Collider[20];

        // TODO: change the name
        private Collider[] m_PLayerColliderHolder = new Collider[1];

        private bool m_Throwing;

        private void Start()
        {
            AnimController.SetInt(HASH_ZOMBIE_ID, 3);
        }

        protected override void Update()
        {
            base.Update();
        }

        public override void RiseFromTheDead()
        {
            base.RiseFromTheDead();
            
            FireAction = new TimedAction(Attack, 0f, 1f);
            StartAttack();
        }

        public override void StartAttack()
        {
            base.StartAttack();
        }

        public override void Attack()
        {
            ThrowNearByObject();
        }

        public override void StopAttack()
        {
            base.StopAttack();
        }

        [Button]
        public void ThrowNearByObject()
        {
            if (m_Throwing)
                return;

            var position = transform.position;

            LayerMask enemyLayerMask =
                (1 << LayerMask.NameToLayer("Enemy"));

            var count = Physics.OverlapSphereNonAlloc(position, ZombieData.AttackRange, m_NearByColliders,
                enemyLayerMask);

            GrabObject(GetClosestObject(count, position));
        }

        // TODO: refactor :)
        public void GrabObject(Transform objTransform)
        {
            if(objTransform == null)
                return;
            
            if (objTransform.TryGetComponent<Zombie>(out var zombie))
            {
                m_Throwing = true;

                zombie.Disable();

                objTransform.SetParent(m_GrabbedPosition);
                objTransform.DOLocalMove(Vector3.zero, 0.25f)
                    .OnComplete(() =>
                    {
                        objTransform.DOJump(Player.PlayerTransform.position, 10, 1, 0.25f)
                            .SetEase(Ease.OutBack)
                            .SetDelay(1f)
                            .OnComplete(
                                () =>
                                {
                                    var count = Physics.OverlapSphereNonAlloc(objTransform.position,
                                        ZombieData.AttackRange,
                                        m_PLayerColliderHolder,
                                        LayerMask.GetMask("Player"));

                                    if (count > 0)
                                    {
                                        PlayerExtensions.GetPlayer().GetDamage(ZombieData.DamagePerSecond);
                                        zombie.Die();
                                    }
                                    else
                                    {
                                        objTransform.SetParent(null);
                                        zombie.Enable();
                                    }

                                    m_Throwing = false;
                                });
                    });
            }
            else
            {
            }
        }

        public void ChargeAndAttack()
        {
        }

        public Transform GetClosestObject(int count, Vector3 origin)
        {
            Transform bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;

            for (var i = 0; i < count; i++)
            {
                var t = m_NearByColliders[i].transform;

                if (t == transform)
                {
                    continue;
                }

                bestTarget = UtilExtensions.GetClosestToPoint(origin, closestDistanceSqr, t);
            }
            
            return bestTarget;
        }
    }
}