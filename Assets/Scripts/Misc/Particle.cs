using System;
using Based.Utility;
using Based2;
using DG.Tweening;
using Events;
using GameStages.EventImplementations;
using Pooling;
using Pooling.EventImplementations;
using Roro.Scripts.GameManagement;
using Unity.VisualScripting;
using UnityCommon.Modules;
using UnityEngine;

namespace Misc
{
    public class Particle : MonoBehaviour, IPoolable<Particle>
    {
        [SerializeField]
        private ParticleType m_ParticleType;

        public ParticleType ParticleType => m_ParticleType;
        public Transform SelfTransform() => GetComponent<Transform>();
        
        public ParticleSystem SelfParticleSystem() => GetComponent<ParticleSystem>();

        private bool m_Disabled;
        private bool m_Enabled;

        private Conditional Cond;

        private void Awake()
        {
            GEM.AddListener<ResetLevelEvent>(OnReset);
            
        }
        
        protected void OnReset(ResetLevelEvent evt)
        {
            DisableSelf();
        }


        public Particle Return(Transform parent = null)
        {
            SelfTransform().position = Vector3.zero;
            SelfTransform().SetParent(parent);
            m_Enabled = false;
            m_Disabled = true;
            return this;
        }

        public Particle Get()
        {
            m_Disabled = false;
            m_Enabled = true;
            gameObject.SetActive(true);

            return this;
        }

        public void SetDuration(float dur)
        {
            var s = SelfParticleSystem().main;
            // s.duration = dur;
        }

        public void MoveToPos(Vector3 pos)
        {
            if(!m_Enabled)
                return;
            
            transform.DOMove(pos, 0.35f);
        }
        
        public void Initialize(Vector3 targetPos)
        {
            SelfTransform().SetParent(null);
            SelfTransform().position = targetPos;
            
            SelfParticleSystem().Play();

            Cond = Conditional.Wait(0.54f).Do(() => DisableSelf());
        }
        
        public void Initialize(Transform parent, bool selfDisable = true)
        {
            SelfTransform().SetParent(parent);
            SelfTransform().position = parent.position;
            
            SelfParticleSystem().Play();

            if(!selfDisable)
                return;
            
            Cond = Conditional.Wait(SelfParticleSystem().main.duration).Do(() => DisableSelf());
        }

        public void Disable()
        {
            DisableSelf();
        }

        private void OnDestroy()
        {
            Cond?.Cancel();
        }

        private void DisableSelf()
        {
            if (m_Disabled || !m_Enabled)
                return;
            
            m_Disabled = true;

            transform.DOKill();
            
            this.GameObject().SetActive(false);
            Cond?.Cancel();
            
            //this.gameObject.SetActive(false);
            
            using var evt = PoolReleaseEvent<Particle>.Get(this);
            evt.SendGlobal();
        }
    }
}
