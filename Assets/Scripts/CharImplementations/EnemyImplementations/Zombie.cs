using CharImplementations.EnemyImplementations.ZombieImplementations;
using DG.Tweening;
using SideMissionManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace CharImplementations.EnemyImplementations
{
    public enum ZombieTarget
    {
        None,
        Player,
        Door
    }

    [RequireComponent(typeof(NavMeshAgent))]
    public class Zombie : Enemy
    {
        protected int HASH_WALK_ANIMATION = Animator.StringToHash("Walking");
        protected int HASH_ATTACK_ANIMATION = Animator.StringToHash("Attacking");
        protected int HASH_ZOMBIE_ID = Animator.StringToHash("ZombieId");

        public ZombieData ZombieData => (ZombieData)m_CharInfo;

        public NavMeshAgent NavMeshAgent;
        public RagdollController RagdollController;

        public ZombieTarget CurrentMovementTarget;
        public ZombieTarget CurrentAttackTarget;

        [ShowInInspector]
        [ReadOnly]
        protected bool m_Attacking;

        [ShowInInspector]
        [ReadOnly]
        protected bool m_Moving;

        protected Transform m_PlayerTransform;
        protected Transform m_DoorTransform;

        protected bool m_Disabled;

        private Renderer m_Renderer;

#if UNITY_EDITOR
        private void OnValidate()
        {
            NavMeshAgent = GetComponent<NavMeshAgent>();
            RagdollController = GetComponent<RagdollController>();
            m_Renderer = GetComponentInChildren<Renderer>();
        }
#endif

        private void Awake()
        {
            if (RagdollController == null)
            {
                NavMeshAgent = GetComponent<NavMeshAgent>();
                RagdollController = GetComponent<RagdollController>();
                m_Renderer = GetComponentInChildren<Renderer>();
            }

            Initialize();

            NavMeshAgent.speed = ZombieData.MovementSpeed;
            m_PlayerTransform = PlayerExtensions.GetPlayer().transform;

            m_DoorTransform = SideMissionController.DoorTransform;
        }

        protected virtual void Update()
        {
            if (m_Attacking)
            {
                FireAction?.Update(Time.deltaTime);
            }

            if (m_Moving)
            {
                Move();
            }
        }

        public virtual void RiseFromTheDead()
        {
        }

        public virtual void StartMovement()
        {
            m_Moving = true;
            NavMeshAgent.enabled = true;
            NavMeshAgent.isStopped = false;

            AnimController.SetBool(HASH_ATTACK_ANIMATION, false);
            AnimController.SetBool(HASH_WALK_ANIMATION, true);
        }

        public override void Move()
        {
            base.Move();

            if (CurrentMovementTarget == ZombieTarget.Player)
            {
                NavMeshAgent.SetDestination(m_PlayerTransform.position);
            }
            else if (CurrentMovementTarget == ZombieTarget.Door)
            {
                NavMeshAgent.SetDestination(m_DoorTransform.transform.position);
            }
        }

        public virtual void StopMovement()
        {
            m_Moving = false;
            NavMeshAgent.isStopped = true;
            NavMeshAgent.enabled = false;

            CurrentMovementTarget = ZombieTarget.None;
            AnimController.SetBool(HASH_WALK_ANIMATION, false);
        }

        public void SetMovementTarget(ZombieTarget target)
        {
            if (m_Disabled || m_Attacking)
                return;

            CurrentMovementTarget = target;
            StartMovement();
        }

        public void SetAttackTarget(ZombieTarget target)
        {
            if (m_Disabled || CurrentAttackTarget == target)
                return;

            CurrentAttackTarget = target;

            if (CurrentAttackTarget == ZombieTarget.None)
            {
                StopAttack();
                return;
            }

            StartAttack();
        }

        public virtual void StartAttack()
        {
            m_Attacking = true;

            m_Moving = false;
            NavMeshAgent.isStopped = true;

            AnimController.SetBool(HASH_ATTACK_ANIMATION, true);
            AnimController.SetBool(HASH_WALK_ANIMATION, false);
        }

        public override void Attack()
        {
        }

        public virtual void StopAttack()
        {
            m_Attacking = false;
            CurrentAttackTarget = ZombieTarget.None;

            AnimController.SetBool(HASH_ATTACK_ANIMATION, false);
        }

        public virtual void Enable()
        {
            m_Disabled = false;
            m_Collider.isTrigger = false;

            SetMovementTarget(ZombieTarget.Player);
        }

        public virtual void Disable()
        {
            m_Disabled = true;
            m_Collider.isTrigger = true;

            StopAttack();
            StopMovement();
        }

        public override void GetDamage(float damage)
        {
            base.GetDamage(damage);
            m_Renderer.material.DOColor(Color.red, 0.5f).SetLoops(2, LoopType.Yoyo);
        }

        public override void Die()
        {
            StopAttack();

            base.Die();
            m_Collider.enabled = false;

            RagdollController.EnableRagdoll();
            transform.DOScale(Vector3.zero, 0.2f).SetDelay(3f);
        }
    }
}