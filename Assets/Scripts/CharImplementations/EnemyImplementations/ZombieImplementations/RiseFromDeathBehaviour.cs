using UnityEngine;

namespace CharImplementations.EnemyImplementations.ZombieImplementations
{
    public class RiseFromDeathBehaviour : StateMachineBehaviour
    {

        // TODO: nooope
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Rise From Death)");
            animator.GetComponent<Zombie>().RiseFromTheDead();
        }
    }
}