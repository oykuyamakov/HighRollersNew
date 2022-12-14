using System;
using System.Collections;
using System.Collections.Generic;
using UnityCommon.Modules;
using UnityEngine;
using UnityEngine.Playables;

namespace AnimationManagement
{
    [RequireComponent(typeof(Animator))]
    public class AnimationController : MonoBehaviour
    {
        private Animator m_Animator => GetComponent<Animator>();

        private Conditional m_Conditional;

        public void SetBool(string animName, bool value)
        {
            m_Animator.SetBool(animName, value);
        }

        public void SetFloat(string name, float value)
        {
            m_Animator.SetFloat(name, value);
        }

        public void SetSpeedMultiplier(string name,float multiplier)
        {
            m_Animator.SetFloat(name,multiplier);
        }

        public void Toggle(string animName, float dur)
        {
            m_Animator.SetBool(animName, true);

            m_Conditional = Conditional.Wait(dur).Do(() => m_Animator.SetBool(animName, false));
        }

        public void Trigger(string animName)
        {
            m_Animator.SetTrigger(animName);
        }

        public void Toggle(string animName, bool value)
        {
            StartCoroutine(ToggleRoutine(animName, value));
        }
        public IEnumerator ToggleRoutine(string animName, bool value)
        {
            m_Animator.SetBool(animName, value);
            yield return new WaitForSeconds(0.1f);
            m_Animator.SetBool(animName, !value);
        }

        private void OnDestroy()
        {
            m_Conditional?.Cancel();
        }
    }
}
