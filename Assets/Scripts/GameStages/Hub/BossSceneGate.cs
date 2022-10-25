using CharImplementations.PlayerImplementation;
using SceneManagement;
using UI.GamePlay.SceneTransition;
using UnityEngine;

namespace GameStages.Hub
{
    public class BossSceneGate : MonoBehaviour
    {
        public bool Entered;

        private void OnTriggerEnter(Collider other)
        {
            if(Entered)
                return;
        
            if (other.transform.GetComponent<Player>())
            {
                SceneChangeUI.Instance.Set("Boss One", SceneId.BossOne, Reset);

                Entered = true;
            }
        
        }

        private void Reset()
        {
            Entered = false;
        }


    }
}
