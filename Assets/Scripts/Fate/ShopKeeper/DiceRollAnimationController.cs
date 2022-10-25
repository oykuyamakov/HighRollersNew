using System.Collections.Generic;
using UnityEngine;

namespace Fate.ShopKeeper
{
    // TODO: to be fully implemented
    public class DiceRollAnimationController : MonoBehaviour
    {
        public int MaxCount = 12; 
        
        public List<DiceObject> DiceObjects = new List<DiceObject>();

        public DiceObject DiceObjectPrefab;

        public int CurrentDiceCount;

        private void OnEnable()
        {
            for (int i = 0; i < MaxCount; i++)
            {
                var diceObject = Instantiate(DiceObjectPrefab, transform);
                diceObject.gameObject.SetActive(false);
                DiceObjects.Add(diceObject);
            }   
        }

        public void AddDice()
        {
            if(CurrentDiceCount >= MaxCount)
                return;
            
            DiceObjects[++CurrentDiceCount].gameObject.SetActive(true);
        }

        public void RemoveDice()
        {
            DiceObjects[CurrentDiceCount--].gameObject.SetActive(false);
        }
    }
}