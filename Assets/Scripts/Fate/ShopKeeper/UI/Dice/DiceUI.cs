using System;
using System.Collections;
using UnityCommon.Modules;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Fate.ShopKeeper.UI
{
    public class DiceUI : MonoBehaviour
    {
        public float AnimationTransitionTime = 0.2f;

        public Image DiceImage;
        
        private FateSettings m_Settings;

        private WaitForSeconds m_WaitForSeconds = new WaitForSeconds(0.2f);
        
        private void Awake()
        {
            m_Settings = FateSettings.Get();
        }

        public int RollDice(int rolledNumber, float duration = 1f)
        {
            Conditional.RepeatNow(AnimationTransitionTime, (int) (duration / AnimationTransitionTime), GetRandomDiceSprite)
                .OnComplete(() =>
                {
                    DiceImage.sprite = m_Settings.DiceSprites[rolledNumber - 1];
                });
            
            return rolledNumber;
        }

        public int RollDice(int rolledNumber, float duration, Action onRollComplete)
        {
            Conditional.RepeatNow(AnimationTransitionTime, (int) (duration / AnimationTransitionTime), GetRandomDiceSprite)
                .OnComplete(() =>
                {
                    DiceImage.sprite = m_Settings.DiceSprites[rolledNumber - 1];
                    onRollComplete?.Invoke();
                });
            
            return rolledNumber;
        }
        
        private void GetRandomDiceSprite()
        {
            var randomIndex = Random.Range(0, m_Settings.DiceSprites.Count);
            DiceImage.sprite = m_Settings.DiceSprites[randomIndex];
        }
    }
}