using System.Collections.Generic;
using AnimationManagement;
using Roro.UnityCommon.Scripts.Runtime.Utility;
using UnityCommon.Modules;
using UnityCommon.Runtime.Utility;
using UnityEngine;
using Utility;

namespace CharImplementations.Interfaces
{
    public interface ICharacter
    {
        public abstract CharType GetCharType();
        public BoolState CanMove { get; set; }
        public BoolState Immune { get; set; }
        public BoolState CanAttack { get; set; }
        public IAction FireAction { get; set; }
        public string CharName { get;  set; }        
        public float MoveSpeed { get;  set; }
        public float Health { get;  set; }
        public void Die();
        public void Move();
        public void ChangeHealth(float amount);
        public void Attack();
        public void OnImmune();
        public void OnImpact(CharType targeted, float damage);
        public void GetDamage(float damage);
        public void GetStunned(float dur);
        public void GetSnared(float dur);
        public void GetKnocked(float dur, Vector3 dir);
        public void GetImmune(float dur);
        public void Dodge(float dodgeDur, float immuneDur);
        public void GetInteracted();
        public void ResetStates();
    }
}
