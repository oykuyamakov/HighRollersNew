using CharImplementations.PlayerImplementation;
using UnityCommon.Runtime.Utility;
using UnityEngine;

namespace CharImplementations.EnemyImplementations.ZombieImplementations
{
    [RequireComponent(typeof(DebugSphere))]
    public class Holder : Zombie
    {
        private DebugSphere m_DebugSphere => GetComponent<DebugSphere>();
        private void AssignTarget()
        {
            // if (Physics.CheckSphere(transform.position, 4, LayerMask.NameToLayer("Player")))
            // {
            //     TargetPosition = Player.PlayerTransform.position;
            // }
            // else
            // {
            //     TargetPosition = GatePosition;
            // }
        }
        
        public override void Move()
        {
            base.Move();
        }

        public override void Attack()
        {
            base.Attack();
        }
    }
}
