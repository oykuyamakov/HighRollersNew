using CharImplementations;
using CharImplementations.PlayerImplementation.EventImplementations;
using CombatManagement.EventImplementations;
using Events;
using Fate.EventImplementations;
using SceneManagement;
using UnityCommon.Modules;
using UnityCommon.Runtime.Utility;
using UnityCommon.Variables;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fate
{
    public class FateEnergyManager : MonoBehaviour
    {
        public static FateSettings Settings => FateSettings.Get();

        public static IntVariable CurrentFateEnergy;

        public static bool IsFateEnergyFull => CurrentFateEnergy >= Settings.MaxEnergy;

        public TimedAction FateEnergyFillingAction;

        private bool m_FateEnergyFlag; // TODO: refactor

        private void OnEnable()
        {
            GEM.AddListener<CharacterDamageEvent>(OnCharacterDamage);
            GEM.AddListener<ProjectileDamageEvent>(OnProjectileDamage);
            GEM.AddListener<GainFateEnergyEvent>(OnGainFateEnergy);
            GEM.AddListener<LoseFateEnergyEvent>(OnLoseFateEnergy);
            GEM.AddListener<ConcludeFateAttackEvent>(OnFateAttackCalled);
            GEM.AddListener<CancelFateAttackEvent>(OnFateAttackCanceled);

            GEM.AddListener<SceneChangeRequestEvent>(OnSceneTransitionRequest);

            CurrentFateEnergy = Variable.Get<IntVariable>("CurrentFateEnergy");

#if UNITY_EDITOR
            Conditional.WaitFrames(5)
                .Do(() =>
                {
                    if (SceneManager.GetSceneByName("BossOne").isLoaded)
                    {
                        InitializeFateEnergyFillingAction();
                    }
                });
#endif
        }

        private void OnSceneTransitionRequest(SceneChangeRequestEvent evt)
        {
            if (evt.sceneId == SceneId.BossOne)
            {
                InitializeFateEnergyFillingAction();
            }
        }

        public void InitializeFateEnergyFillingAction()
        {
            FateEnergyFillingAction =
                new TimedAction(OnFateEnergyFillingAction, 0, Settings.IncrementalFillingInterval);
        }

        private void Update()
        {
            if (m_FateEnergyFlag || FateEnergyFillingAction == null)
                return;

            FateEnergyFillingAction.Update(Time.deltaTime);
        }

        private void OnGainFateEnergy(GainFateEnergyEvent evt)
        {
            GainFateEnergy(evt.Amount);
        }

        private void GainFateEnergy(int amount)
        {
            if(m_FateEnergyFlag)
                return;
            
            CurrentFateEnergy.Value += amount;
            CurrentFateEnergy.Value = Mathf.Clamp(CurrentFateEnergy.Value, 0, Settings.MaxEnergy);

            if (CurrentFateEnergy.Value == Settings.MaxEnergy)
            {
                m_FateEnergyFlag = true;

                using (var evt = FateEnergyFullEvent.Get())
                {
                    evt.SendGlobal();
                }
            }
        }

        private void OnLoseFateEnergy(LoseFateEnergyEvent evt)
        {
            LoseFateEnergy(evt.Amount);
        }

        private void LoseFateEnergy(int amount)
        {
            CurrentFateEnergy.Value -= amount;
            CurrentFateEnergy.Value = Mathf.Clamp(CurrentFateEnergy.Value, 0, Settings.MaxEnergy);
        }

        private void OnFateEnergyFillingAction()
        {
            GainFateEnergy(Settings.IncrementalFillingAmount);
        }

        private void OnCharacterDamage(CharacterDamageEvent evt)
        {
            GainFateEnergy(evt.CharType == CharType.Player
                ? Settings.DamageEnergyFillAmount
                : Settings.AttackEnergyFillAmount);
        }

        private void OnProjectileDamage(ProjectileDamageEvent evt)
        {
            GainFateEnergy(Settings.AttackEnergyFillAmount);
        }
        
        private void OnFateAttackCalled(ConcludeFateAttackEvent evt)
        {
            CurrentFateEnergy.Value = 0;
            m_FateEnergyFlag = false;
        }
        
        private void OnFateAttackCanceled(CancelFateAttackEvent evt)
        {
            CurrentFateEnergy.Value = Settings.MaxEnergy;
        }

        private void OnDisable()
        {
            GEM.RemoveListener<CharacterDamageEvent>(OnCharacterDamage);
            GEM.RemoveListener<GainFateEnergyEvent>(OnGainFateEnergy);
            GEM.RemoveListener<LoseFateEnergyEvent>(OnLoseFateEnergy);
        }
    }
}