using System.Collections;
using System.Text;
using Based.Utility;
using Based2.TimeManagement.Core;
using Events;
using MoreMountains.NiceVibrations;
using Roro.Scripts.GameManagement.EventImplementations;
using Roro.Scripts.Serialization;
using Roro.Scripts.UI;
using UnityCommon.Runtime.UI;
using UnityCommon.Variables;
using UnityEngine;

namespace Roro.Scripts.GameManagement
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private BoolVariable m_GameIsRunning;

        // [SerializeField]
        // private HCTopUI m_TopUI;

       // private static GeneralSettings m_GeneralSettings => GeneralSettings.Get();
        private static SerializationWizard m_SerializationWizard => SerializationWizard.Default;

        private static int m_Level
        {
            get => m_SerializationWizard.GetInt("level", 0);
            set => m_SerializationWizard.SetInt("level", value);
        }

        private float m_Duration;

        private GameObject m_LastLevel;

        private bool m_StartedTimer = false;

        private long m_FinishTime;

        private Coroutine m_TimerRoutine;

        private void Awake()
        {
            GEM.AddListener<LevelEndEvent>(OnLevelEnd);
            GEM.AddListener<LevelStartEvent>(OnLevelStart);
            GEM.AddListener<EndPanelEventEvent>(OnEndPanel);

            StartCoroutine(StartCountdown());
        }

        private void InitializeLevel()
        {
        }
        
        private void SetTime()
        {
            m_FinishTime = (long) (m_Duration * 60000);
            m_StartedTimer = true;
        }


        private float wait = 0.06f;

        public IEnumerator StartCountdown()
        {
            while (true)
            {
                yield return new WaitForSeconds(wait);


                if (m_GameIsRunning.Value)
                {
                    var sb = new StringBuilder();
                    TimeManager.FormatShort(m_FinishTime, sb);
                    //m_TopUI.TrySetTimerText(sb.ToString());
                    m_FinishTime -= 60;
                }

                if (m_FinishTime > 0 || !m_GameIsRunning.Value || !m_StartedTimer)
                    continue;

                m_GameIsRunning.Value = false;
                m_StartedTimer = false;
                
                using var levelendEvent = LevelEndEvent.Get();
                levelendEvent.SendGlobal();
            }
        }

        private void OnDestroy()
        {
            GEM.RemoveListener<LevelEndEvent>(OnLevelEnd);
            GEM.RemoveListener<LevelStartEvent>(OnLevelStart);
            GEM.RemoveListener<EndPanelEventEvent>(OnEndPanel);
        }

        private void OnLevelEnd(LevelEndEvent evt)
        {
            m_GameIsRunning.Value = false;
        }

        private void OnEndPanel(EndPanelEventEvent evt)
        {
            
        }

        private void OnLevelStart(LevelStartEvent evt)
        {
            MMVibrationManager.Haptic(HapticTypes.Success);

            FadeInOut.Instance.DoTransition(InitializeLevel, 1f, Color.black);
        }
    }
}