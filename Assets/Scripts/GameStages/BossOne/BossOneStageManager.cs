using System;
using System.Collections.Generic;
using CharImplementations;
using CharImplementations.PlayerImplementation;
using CombatManagement.EventImplementations;
using CombatManagement.ProjectileManagement;
using DG.Tweening;
using Events;
using GameStages.EventImplementations;
using SettingImplementations;
using UnityCommon.Runtime.Utility;
using UnityEngine;

namespace GameStages.BossOne
{
    public class BossOneStageManager : GameStageManager
    {
        [SerializeField]
        private List<Transform> m_MissileLids;

        private Collider m_Collider => GetComponent<Collider>();
        
        private PhaseOneValues PhaseOneValues => BossOneSettings.Get().PhaseOneValues;

        private int m_StageIndex = 1;
        private int m_FiredMissileCount = 0;
        private int m_LidIndex = 0;

        private TimedAction m_MissileFireAction;

        private void Awake()
        {
            GEM.AddListener<ResetLevelEvent>(Reset);
            GEM.AddListener<PhaseChangeEvent>(OnStageChangeEvent);
        }

        private void FixedUpdate()
        {
            m_MissileFireAction?.Update(Time.deltaTime);
        }

        public void Reset(ResetLevelEvent evt)
        {
            m_MissileFireAction.Pause();
            m_Collider.enabled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<Player>(out var pl))
                return;

            m_Collider.enabled = false;

            m_StageIndex = 1;
            
            OnStageStart();
                
            m_MissileFireAction = new TimedAction(OnMissileFire, PhaseOneValues.MissileRepeatValues.InitialDelay, PhaseOneValues.MissileRepeatValues.Rate);
        }

        private void OnStageChangeEvent(PhaseChangeEvent evt)
        {
            if (evt.PhaseIndex > 1)
            {
                m_MissileFireAction.Pause();
            }
            else
            {
                m_MissileFireAction?.Resume();

            }
        }
        
        private void OnStageStart()
        {
            using var evt = PhaseChangeEvent.Get(1);
            evt.SendGlobal();

            // using var musicEvent = SoundPlayEvent.Get(GeneralSettings.Get().BossSound);
            // musicEvent.SendGlobal();
            
            m_StageIndex++;
        }

        public void OnMissileFire()
        {
            if (PhaseOneValues.PhaseOneAttack != PhaseOneAttacks.Missile)
            {
                return;
            }
            
            var t = m_MissileLids[m_LidIndex % m_MissileLids.Count];
            var pos = t.position;

            m_LidIndex++;
            
            using var evt = GetProjectileEvent.Get(ProjectileType.Missile);
            evt.SendGlobal();
            var missile = evt.Projectile;

            var layerMask = 1 <<  LayerMask.NameToLayer("Player") | 1 <<  
                LayerMask.NameToLayer("ProjectilePlayer") | 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("Level");

            Debug.Log("2");

            missile.SelfCollider().enabled = false;
            missile.Initialize(
                pos + Vector3.up ,
                Player.PlayerTransform.position, 5, CharType.Player, layerMask, "ProjectileEnemy");
            missile.InitializeProjectileInfoParameters(BossOneSettings.Get().PhaseOneValues.MissileMovement, Player.PlayerTransform,false);
                
            missile.SelfCollider().enabled = true;

            var seq = DOTween.Sequence();
            // seq.Append(t.DORotate(Vector3.up * 50f, 0.1f));
            // seq.Append(t.DOMove(t.position + Vector3.down, 0.1f).OnComplete(() =>
            // {
            //
            //     using var evt = GetProjectileEvent.Get(ProjectileType.Missile);
            //     evt.SendGlobal();
            //     var missile = evt.Projectile;
            //
            //     var layerMask = 1 <<  LayerMask.NameToLayer("Player") | 1 <<  
            //         LayerMask.NameToLayer("ProjectilePlayer") | 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("Level");
            //
            //     missile.SelfCollider().enabled = false;
            //     missile.Initialize(
            //          pos + Vector3.up,
            //          Player.PlayerTransform.position, 5, CharType.Player, layerMask, "ProjectileEnemy");
            //     missile.InitializeMovement(BossOneSettings.Get().PhaseOneValues.MissileMovement, Player.PlayerTransform,false);
            //     
            //     missile.SelfCollider().enabled = true;
            //
            //     //missile.FollowTarget(Player.PlayerTransform);
            //     //missile.SelfCollider().enabled = true;
            //
            //     t.DOMove(pos, .9f);
            //
            // }));
            
            using var evt2 = SoundPlayEvent.Get(BossOneSettings.Get().MissileFireSound);
            evt2.SendGlobal();

            m_FiredMissileCount++;

            if (m_FiredMissileCount > PhaseOneValues.MissileRepeatValues.ArbitraryCount - 1)
            {
                m_MissileFireAction.Period = PhaseOneValues.MissileRepeatValues.Cooldown;
                m_FiredMissileCount = 0;
            }
            else
            {
                m_MissileFireAction.Period = PhaseOneValues.MissileRepeatValues.Rate;
            }
        }
    }
}
