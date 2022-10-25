using UnityCommon.Runtime.Utility;
using Random = UnityEngine.Random;

namespace CharImplementations.EnemyImplementations.ZombieImplementations
{
    public class Holder : Zombie
    {
        private void Start()
        {
            AnimController.SetInt(HASH_ZOMBIE_ID, 0);
        }

        protected override void Update()
        {
            base.Update();
        }

        public override void RiseFromTheDead()
        {
            base.RiseFromTheDead();

            FireAction = new TimedAction(Attack, 0f, 1f);

            var rand = Random.Range(0, 2);
            SetMovementTarget(rand == 0 ? ZombieTarget.Player : ZombieTarget.Door);
        }
        
        public override void StartAttack()
        {
            base.StartAttack();

            if (CurrentAttackTarget == ZombieTarget.Player)
            {
                PlayerExtensions.GetPlayer().GetSnared(true);
            }
            else
            {
                AnimController.SetBool("Door", true);
            }
        }

        public override void Attack()
        {
            if (CurrentAttackTarget == ZombieTarget.Player)
            {
                PlayerExtensions.GetPlayer().GetDamage(ZombieData.DamagePerSecond);
            }
        }

        public override void StopAttack()
        {
            base.StopAttack();
            PlayerExtensions.GetPlayer().GetSnared(false);
            AnimController.SetBool("Door", false);
        }
    }
}