using System;
using System.Collections;
using CharImplementations.PlayerImplementation.EventImplementations;
using Cinemachine;
using Events;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CameraManagement
{
    public class CameraEffectController : MonoBehaviour
    {
        [SerializeField] 
        private CinemachineVirtualCamera m_CinCam;

        private Coroutine m_ShakeRoutine;

        private Vector2 DefaultAmp;

        private void Awake()
        {
            GEM.AddListener<PlayerHealthChangeEvent>(OnPlayerDamage);
        }

        private void OnDestroy()
        {
            GEM.RemoveListener<PlayerHealthChangeEvent>(OnPlayerDamage);
        }

        [Button]
        public void StartCameraShake(float amp, float time) {
            if (m_ShakeRoutine != null) {
                StopCoroutine(m_ShakeRoutine);
            }
            m_ShakeRoutine = StartCoroutine(Shaker(amp, time));
        }

        private IEnumerator Shaker(float Amp, float TimeForShake) {
            var perlin = m_CinCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            var TimeDiff = Amp / TimeForShake;
            perlin.m_AmplitudeGain = Amp;
            perlin.m_FrequencyGain = 1;
            while (TimeForShake != 0) {
                TimeForShake = Mathf.MoveTowards(TimeForShake, 0, Time.deltaTime);
                perlin.m_AmplitudeGain = Mathf.MoveTowards(perlin.m_AmplitudeGain, DefaultAmp.x, TimeDiff * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            TimeForShake = 0;
            perlin.m_AmplitudeGain = DefaultAmp.x;
            perlin.m_FrequencyGain = DefaultAmp.y;
        }

        public void OnPlayerDamage(PlayerHealthChangeEvent evt)
        {
            StartCameraShake(10, 0.5f);
        }
    }
}
