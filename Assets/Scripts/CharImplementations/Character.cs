using System;
using System.Collections.Generic;
using AnimationManagement;
using CharImplementations.Data;
using CharImplementations.Interfaces;
using CharImplementations.PlayerImplementation.EventImplementations;
using DG.Tweening;
using Events;
using GameStages.EventImplementations;
using Roro.UnityCommon.Scripts.Runtime.Utility;
using UnityCommon.Runtime.Utility;
using UnityEngine;
using Utility;

namespace CharImplementations
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AnimationController))]
    public abstract class Character : MonoBehaviour, ICharacter
    {
        #region Fields
        
        [SerializeField]
        protected CharInfo m_CharInfo;
        public abstract CharType GetCharType();
        public BoolState CanMove { get; set; }
        public BoolState Immune { get; set; }
        public BoolState CanAttack { get; set; }
        public virtual float MoveSpeed {get => m_CharInfo.MovementSpeed; set => m_CharInfo.MovementSpeed = value; }
        public string CharName { get; set; }
        public float Health { get => m_Health.CurrentValue; set => m_Health.Change(value); }
        
        public IAction FireAction { get; set; }
        protected Rigidbody m_Rigidbody => GetComponent<Rigidbody>();
        protected Transform m_Transform => GetComponent<Transform>();
        protected Collider m_Collider => GetComponent<Collider>();
        protected AnimationController AnimController => GetComponent<AnimationController>();
        
        protected FloatState m_Health;
        
        #endregion
        
        protected virtual void Initialize()
        {
            CharName = m_CharInfo.Name;
            MoveSpeed = m_CharInfo.MovementSpeed;

            m_Health = new FloatState(m_CharInfo.HealthMax, 
                new SpecialCondition<float>(0,Die, ComparisionMethod.LesserOnce));
            Immune = new BoolState(false);
            CanMove = new BoolState(true);
            CanAttack = new BoolState(true);
        }

        public virtual void Die()
        {
            AnimController.SetBool("Die", true);
            CanAttack.Change(false);
            CanMove.Change(false);
        }

        public virtual void Move()
        {
            
        }

        protected virtual void OnResetLevel(ResetLevelEvent evt)
        {
            MoveSpeed = m_CharInfo.MovementSpeed;
            // TODO
            m_Health = new FloatState(m_CharInfo.HealthMax, 
                new SpecialCondition<float>(0,Die, ComparisionMethod.LesserOnce));

            FireAction?.Pause();
            Immune.Change(false);
            CanMove.Change(true);
            CanAttack.Change(true);
        }

        public virtual void OnImpact(CharType targeted, float damage)
        {
            if (targeted == GetCharType())
            {
                GetDamage(damage);
            }
        }

        public virtual void GetDamage(float damage)
        {
            if(Immune.CurrentValue)
                return;
            
            damage = -Mathf.Abs(damage);
            ChangeHealth(damage);

            using var evt = CharacterDamageEvent.Get(GetCharType());
        }

        public virtual void GetStunned(float dur)
        {
            CanMove.Toggle(false, dur);
        }

        public virtual void GetSnared(float dur)
        {
            CanAttack.Toggle(false, dur);
            CanMove.Toggle(false, dur);
        }
        public virtual void GetKnocked(float dur, Vector3 dir)
        {
            CanMove.Toggle(false, dur);
            m_Rigidbody.AddForce(dir, ForceMode.Impulse);
        }
        
        public virtual void GetImmune(float dur)
        {
            
        }

        public virtual void OnImmune()
        {
            
        }

        public virtual void OnImmuneEnd()
        {
            
        }
       
        public virtual void Dodge(float dodgeDur, float immuneDur)
        {
            Immune.Toggle(true, immuneDur);
        }
        
        public virtual void ChangeHealth(float amount)
        {
            Health += amount;
        }

        public virtual void Attack()
        {
        }

        public virtual void GetInteracted()
        {
        }
        
        public virtual void ResetStates()
        {
            Immune.Reset();
            CanMove.Reset();
            CanAttack.Reset();
            m_Health.Reset();
        }
    }

    public enum CharType
    {
        Player,
        Npc,
        Enemy
    }
}
