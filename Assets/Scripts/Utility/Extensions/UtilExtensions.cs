using DG.Tweening;
using UnityEngine;

namespace Utility.Extensions
{
    public static class UtilExtensions
    {
        public static void Toggle(this CanvasGroup canvasGroup, bool visible, float duration)
        {
            canvasGroup.DOFade(visible ? 1 : 0, duration);
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
        
        /// <summary>
        /// if denominator is 0, returns numerator
        /// </summary>
        /// <param name="Numerator"></param>
        /// <param name="Denominator"></param>
        /// <returns></returns>
        public static float SafeDivision(this float Numerator, float Denominator)
        {
            return (Denominator == 0) ? Numerator : Numerator / Denominator;
        }
    }
}