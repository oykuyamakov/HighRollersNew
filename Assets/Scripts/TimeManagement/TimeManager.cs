using Events;
using SettingImplementations;
using Sirenix.OdinInspector;
using TimeManagement.EventImplementations;
using UnityCommon.Singletons;
using UnityEngine;

namespace TimeManagement
{
    public class TimeManager : SingletonBehaviour<TimeManager>
    {
        [ShowInInspector]
        public static int Hour { get; private set; }
        
        [ShowInInspector]
        public static int Minute { get; private set; }

        public float MinuteToRealTime => m_Settings.MinuteToRealTime;
        public int DayHour => m_Settings.DayHour;
        public int NightHour => m_Settings.NightHour;

        private GeneralSettings m_Settings;

        private float m_TimeElapsed;

        public static void SetTime(int hour, int minute)
        {
            Hour = hour;
            Minute = minute;
        }
        
        private void Awake()
        {
            m_Settings = GeneralSettings.Get();

            m_TimeElapsed = MinuteToRealTime;
        }

        private void Update()
        {
            m_TimeElapsed -= Time.deltaTime;

            if (m_TimeElapsed <= 0)
            {
                Minute++;
                CallMinuteEvent();
                
                if (Minute >= 60)
                {
                    Minute = 0;
                    Hour = Hour >= 23 ? 0 : Hour + 1;
                    
                    CallHourEvent();
                }

                m_TimeElapsed = MinuteToRealTime;
            }
        }

        private void CallHourEvent()
        {
            using var hourEvent = HourChangedEvent.Get().SendGlobal();

            if (Hour == DayHour)
            {
                using var dayEvent = TransitionToDayEvent.Get().SendGlobal();
            }
            else if (Hour == NightHour)
            {
                using var nightEvent = TransitionToNightEvent.Get().SendGlobal();
            }
        }

        private void CallMinuteEvent()
        {
            using var hourEvent = MinuteChangedEvent.Get().SendGlobal();
        }
    }
}