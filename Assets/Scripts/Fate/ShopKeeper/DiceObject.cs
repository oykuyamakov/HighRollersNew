using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Fate.ShopKeeper
{
    public class DiceObject : MonoBehaviour
    {
        private Transform m_Transform;

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_Transform = transform;
        }
#endif
        
        [Button]
        public int RollDice(int rolledNumber, float duration = 1f)
        {
            Vector3 targetRotation = Dice.GetDiceRotationForSide(rolledNumber);
            Vector3 randomRotation = new Vector3(Random.Range(60, 360), Random.Range(60, 360), Random.Range(60, 360));

            // TODO: will be better
            Sequence seq = DOTween.Sequence();
            seq.Append(m_Transform.DOShakeRotation(duration * 0.65f, randomRotation, 10, 45));
            seq.Append(m_Transform.DORotate(targetRotation,duration * 0.35f).SetEase(Ease.OutQuad));
                
            return rolledNumber;
        }
        
    }
}