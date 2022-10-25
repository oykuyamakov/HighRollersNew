using System;
using CharImplementations;
using DG.Tweening;
using Events;
using GameStages.EventImplementations;
using Misc.EventImplementations;
using Roro.Scripts.GameManagement;
using SettingImplementations;
using UnityCommon.Modules;
using UnityCommon.Runtime.Utility;
using UnityEngine;

namespace GameStages.BossOne.HitArea
{
    public class HitArea : MonoBehaviour
    {
        // [SerializeField] 
        // private ParticleSystem m_HitParticle;

        [SerializeField] 
        private Material m_Mat;
        private Material m_OriginMat;
        private Collider m_Collider => GetComponent<Collider>();

        private Material m_SelfMat;
        private Material m_CloneElectric;

        private CharType m_CharType;

        private float m_DamageAmount;

        private Color m_StartColor;

        private bool m_Enabled;        
        private bool m_CanDamage;
        private bool m_TargetIsInside;
        private bool m_InstantDamage;

        private Conditional m_Conditional;

        private TimedAction m_DamageAction;
        private Character m_CharacterToDamage;

        private void TryDamage()
        {
            if (m_CharacterToDamage != null)
            {
                m_CharacterToDamage.OnImpact(CharType.Player, -m_DamageAmount);
                
                using var evt = ParticleSpawnEvent.Get(ParticleType.SphereExplosion);
                evt.SendGlobal();
                evt.Particle.Initialize(transform.position + Vector3.up);
            }
        }


        private void Awake()
        {
            // m_SelfMat = GetComponent<Renderer>().material = new Material(GetComponent<Renderer>().materials[1]);
            // m_StartColor = m_SelfMat.color;
            m_CloneElectric = new Material(GetComponent<Renderer>().materials[1]);
            m_OriginMat = GetComponent<Renderer>().materials[1];
            GetComponent<Renderer>().materials[1] = m_CloneElectric;
            
            GEM.AddListener<ResetLevelEvent>(OnResetEvent);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponent<Character>(out var chara))
            {
                if (!m_InstantDamage)
                {
                    m_CharacterToDamage = chara;
                    return;
                }
                if (chara.GetCharType() == m_CharType)
                {
                    if (!m_CanDamage)
                        return;
                    m_CanDamage = false;

                    chara.GetDamage(-m_DamageAmount);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            m_CharacterToDamage = null;
        }

        private void OnResetEvent(ResetLevelEvent evt)
        {
            //m_HitParticle.Stop();
            m_Collider.enabled = false;
            //m_SelfMat.color = m_StartColor;
            m_CanDamage = false;
            m_Conditional?.Cancel();
        }

        public void ActivateBuildUpDamage(CharType charType, float damage, float dur, float waitDur, Action onComplete = null)
        {
            m_DamageAmount = damage;
            m_CloneElectric = m_Mat;
            m_CharType = charType;

            m_Collider.enabled = true;
            m_DamageAction = new TimedAction(TryDamage, waitDur, 0.8f);

            m_CanDamage = true;
            m_Collider.enabled = true;

            
            Conditional.For(waitDur + dur).Do(() =>
            {
                m_DamageAction.Update(Time.deltaTime);
            }).OnComplete(() =>
            {
                m_CloneElectric = m_OriginMat;
                m_CanDamage = false;
                m_Collider.enabled = false;
                GetComponent<Renderer>().materials[1] = m_SelfMat;
                    
                onComplete?.Invoke();
            });
        }

        public void ActivateInstantDamage(CharType charType, float damage, float dur, float indicatorDur, Action onComplete = null)
        {
            m_DamageAmount = damage;
            m_SelfMat.color = Color.white;
            m_CharType = charType;
            m_CanDamage = true;
            m_InstantDamage = true;
            m_CloneElectric = m_Mat;

            
            m_SelfMat.DOColor(BossOneSettings.Get().PhaseOneValues.HitIndicatorColor, indicatorDur).OnComplete(() =>
            {                
            
                m_Collider.enabled = true;
                m_Enabled = true;
                //m_HitParticle.Play();

                m_Conditional = Conditional.Wait(dur).OnComplete(() =>
                {
                    m_Collider.enabled = false;
                    onComplete?.Invoke();
                    m_SelfMat.color = m_StartColor;
                    m_CanDamage = false;
                });
            });
        }
    }
}