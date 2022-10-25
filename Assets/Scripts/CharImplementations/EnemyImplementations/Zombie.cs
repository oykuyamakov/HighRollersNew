using System;
using CharImplementations.PlayerImplementation;
using CombatManagement;
using CombatManagement.EventImplementations;
using CombatManagement.ProjectileManagement;
using CombatManagement.WeaponImplementation;
using DG.Tweening;
using Events;
using Sirenix.OdinInspector;
using UnityCommon.Runtime.Extensions;
using UnityCommon.Runtime.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;
using Random = System.Random;

namespace CharImplementations.EnemyImplementations
{
    public class Zombie : Enemy
    {
        //protected Vector3 TargetPosition => Player.PlayerTransform;

        protected Vector3 GatePosition;

        protected bool m_Walking;

        private void Awake()
        {
            FireAction = new TimedAction(Attack, UnityEngine.Random.Range(1f,3f), UnityEngine.Random.Range(1f,3f));
            Initialize();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Update()
        {
            FireAction.Update(Time.deltaTime);
        }

        public override void Move()
        {
            base.Move();
            
            transform.LookAt(Player.PlayerTransform.position);
            
            if (m_Walking)
            {
                var dir = (m_Rigidbody.position.WithY(2) - Player.PlayerTransform.position.WithY(2)).normalized;
                m_Rigidbody.MovePosition(m_Rigidbody.position + (-dir * (MoveSpeed * Time.deltaTime)));
            }
            
        }

        public override void GetDamage(float damage)
        {
            base.GetDamage(damage);

            GetComponent<Material>().DOColor(Color.red, 0.5f).SetLoops(2, LoopType.Yoyo);
        }

        public override void Attack()
        {
          

            m_Walking = !m_Walking;
        }

        public override void Die()
        {
            base.Die();
            m_Collider.enabled = false;
            transform.DOScale(Vector3.zero, 0.2f);
        }
    }
}