using System;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Roro.Scripts.UI.UITemplates.UITemplateImplementations
{
    [RequireComponent(typeof(Slider))]
    public class SliderTemplate : UITemplate<float>
    {
        private Slider m_Slider => GetComponent<Slider>();

        public override void Set(float val)
        {
            m_Slider.value = val;
        }

        public void AnimatedSet(float value, float dur, float maxVal = 100)
        {
            var newVal = math.remap(0, 100, 0, 1, value);
            DOTween.To(val => m_Slider.value = val, m_Slider.value, newVal, dur);
        }

        public override void Enable()
        {
            m_Slider.enabled = true;
        }

        public override void Disable()
        {
            m_Slider.enabled = false;
        }

        public override UIElementType GetElementType()
        {
            throw new System.NotImplementedException();
        }
    }
}