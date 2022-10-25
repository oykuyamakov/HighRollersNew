using System.Collections.Generic;
using Fate.ShopKeeper.UI;
using Promises;
using UnityEngine;

namespace Fate.ShopKeeper
{
    public class DiceRollAnimationController : MonoBehaviour
    {
        public List<DiceUI> DiceUis = new List<DiceUI>();

        public DiceUI DiceUIPrefab;

        public int CurrentDiceCount;

        private Promise<int> m_AnimationPromise;

        private void OnEnable()
        {
            for (int i = 0; i < DiceSelectionController.MaxCount; i++)
            {
                var diceUI = Instantiate(DiceUIPrefab, transform);
                diceUI.gameObject.SetActive(false);
                DiceUis.Add(diceUI);
            }
        }

        public void AddDice()
        {
            if (CurrentDiceCount >= DiceSelectionController.MaxCount)
                return;

            DiceUis[CurrentDiceCount++].gameObject.SetActive(true);
        }

        public void RemoveDice()
        {
            DiceUis[--CurrentDiceCount].gameObject.SetActive(false);
        }

        public Promise<int> RollDiceAnim()
        {
            m_AnimationPromise = Promise<int>.Create();

            RollDice(0, 0);

            return m_AnimationPromise;
        }

        private void RollDice(int index, int totalRolled)
        {
            if (index >= CurrentDiceCount)
            {
                m_AnimationPromise.Complete(totalRolled);
                return;
            }

            var rolled = Dice.RollADie();
            totalRolled += rolled;

            DiceUis[index].RollDice(rolled, 1f,
                () => RollDice(index + 1, totalRolled));
        }

        private int RollDiceAll()
        {
            var totalRolled = 0;

            for (var i = 0; i < CurrentDiceCount; i++)
            {
                totalRolled += DiceUis[i].RollDice(Dice.RollADie(), 1f);
            }

            return totalRolled;
        }
    }
}


// using System.Collections.Generic;
// using UnityEngine;
//
// namespace Fate.ShopKeeper
// {
//     // TODO: to be fully implemented
//     public class DiceRollAnimationController : MonoBehaviour
//     {
//         public int MaxCount = 12; 
//         
//         public List<DiceObject> DiceObjects = new List<DiceObject>();
//
//         public DiceObject DiceObjectPrefab;
//
//         public int CurrentDiceCount;
//
//         private void OnEnable()
//         {
//             for (int i = 0; i < MaxCount; i++)
//             {
//                 var diceObject = Instantiate(DiceObjectPrefab, transform);
//                 diceObject.gameObject.SetActive(false);
//                 DiceObjects.Add(diceObject);
//             }   
//         }
//
//         public void AddDice()
//         {
//             if(CurrentDiceCount >= MaxCount)
//                 return;
//             
//             DiceObjects[++CurrentDiceCount].gameObject.SetActive(true);
//         }
//
//         public void RemoveDice()
//         {
//             DiceObjects[CurrentDiceCount--].gameObject.SetActive(false);
//         }
//     }
// }