using Fate.EventImplementations;
using UnityEngine;
using UnityEngine.UI;
using Events;
using InventoryManagement;
using SceneManagement;
using UnityCommon.Modules;
using UnityCommon.Runtime.Variables;
using UnityCommon.Variables;
using UnityEngine.SceneManagement;
using Utility.Extensions;

namespace Fate
{
    // TODO : refactor!!!
    public class FateUIManager : MonoBehaviour
    {
        public InventorySystem FateInventorySystem;

        public IntVariable CurrentFateEnergy;

        public CanvasGroup FateEnergyCanvasGroup;

        public Image FateEnergyBar;

        public Image FateIcon;

        public Button ExitButton;

        private FateSettings m_Settings;

        private void OnEnable()
        {
            GEM.AddListener<ToggleFateEditorUIEvent>(OnToggleFateEditor); // FATE INVENTORY 
            GEM.AddListener<FateEnergyFullEvent>(OnFateAttackReady);
            GEM.AddListener<ConcludeFateAttackEvent>(OnConcludeFateAttack);
            CurrentFateEnergy.AddListener<ValueChangedEvent<int>>(OnFateEnergyModified);
            GEM.AddListener<SceneChangeRequestEvent>(OnSceneTransitionRequest);

            ExitButton.onClick.AddListener(OnExit);

            m_Settings = FateSettings.Get();

#if UNITY_EDITOR
            Conditional.WaitFrames(5)
                .Do(() =>
                {
                    if (SceneManager.GetSceneByName("BossOne").isLoaded)
                    {
                        FateEnergyCanvasGroup.Toggle(true, 0.25f);
                    }
                });
#endif
        }

        private void OnSceneTransitionRequest(SceneChangeRequestEvent evt)
        {
            FateEnergyCanvasGroup.Toggle(evt.sceneId == SceneId.BossOne, 0.25f);
        }

        private void OnToggleFateEditor(ToggleFateEditorUIEvent evt)
        {
            FateInventorySystem.ToggleInventoryUI(evt.Visible);
        }

        private void OnFateEnergyModified(ValueChangedEvent<int> evt)
        {
            FateEnergyBar.fillAmount = (float)CurrentFateEnergy.Value / m_Settings.MaxEnergy;
        }

        private void OnFateAttackReady(FateEnergyFullEvent evt)
        {
            // this should never happen, but just in case and for testing purposes
            if (evt.RolledModule == null)
            {
                return;
            }

            UpdateModuleIcon(evt.RolledModule.Data.ModuleIcon);
        }

        private void OnConcludeFateAttack(ConcludeFateAttackEvent evt)
        {
            UpdateModuleIcon(null);
        }

        private void UpdateModuleIcon(Sprite icon)
        {
            FateIcon.sprite = icon;
        }

        private void OnExit()
        {
            using var evt = ToggleFateEditorUIEvent.Get(false).SendGlobal();
        }

        private void OnDisable()
        {
            ExitButton.onClick.RemoveListener(OnExit);
            GEM.RemoveListener<FateEnergyFullEvent>(OnFateAttackReady);
            CurrentFateEnergy.RemoveListener<ValueChangedEvent<int>>(OnFateEnergyModified);
        }
    }
}