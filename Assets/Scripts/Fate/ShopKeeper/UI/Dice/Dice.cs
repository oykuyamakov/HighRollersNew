using UnityEngine;

namespace Fate.ShopKeeper
{
    public static class Dice
    {
        public static int RollADie(int numberOfSides = 6)
        {
            return Random.Range(1, numberOfSides);
        }

        public static int RollACoupleOfDice(int diceCount, int numberOfSides = 6)
        {
            int result = 0;

            for (int i = 0; i < diceCount; i++)
            {
                result += RollADie(numberOfSides);
            }

            return result;
        }

        // TODO: i don't like the name
        public static Vector3 GetDiceRotationForSide(int rolledNumber)
        {
            Vector3 targetRotation;
            
            switch (rolledNumber)
            {
                case 2:
                    targetRotation = new Vector3(0f, -90f, 0f);
                    break;
                
                case 3:
                    targetRotation = new Vector3(90f, 0f, 0f);
                    break;
                
                case 4:
                    targetRotation = new Vector3(-90f, 0f, 0f);
                    break;
                
                case 5:
                    targetRotation = new Vector3(0f, 90f, 0f);
                    break;
                
                case 6:
                    targetRotation = new Vector3(0f, 180f, 0f);
                    break;
                
                default:
                    targetRotation = Vector3.zero;
                    break;
            }

            return targetRotation;
        }
    }
}