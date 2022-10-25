using System;
using System.Collections.Generic;
using AnimationManagement;
using CharImplementations.NPCImplementation;
using Events;
using Sirenix.OdinInspector;
using TimeManagement;
using TimeManagement.EventImplementations;
using UnityCommon.Modules;
using UnityCommon.Runtime.Collections;
using UnityEngine;
using UnityEngine.AI;
using Utility.Extensions;

namespace CharImplementations.NPCImplementations
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AnimationController))]
    public class NPCScheduleController : MonoBehaviour
    {
        public const int HANGOUT_DURATION = 60;

        public ScheduledActionCategory CurrentActionCategory;

        [ShowIf("@CurrentActionCategory == ScheduledActionCategory.Location")]
        public NPCLocationActionType CurrentLocationActionType;

        [ShowIf("@CurrentActionCategory == ScheduledActionCategory.Player")]
        public NPCPlayerActionType CurrentPlayerActionType;

        public NPCSchedule Schedule;

        public NavMeshAgent Agent;

        public AnimationController AnimationController;

        public PriorityList<ScheduleItem> CurrentSchedule = new PriorityList<ScheduleItem>();

        public Dictionary<TimeData, ScheduleItem> ScheduledActions = new Dictionary<TimeData, ScheduleItem>();

        // TODO: to be implemented
        public Dictionary<TimeData, ScheduleItem> IntervaledActions = new Dictionary<TimeData, ScheduleItem>();

        [ShowInInspector]
        [ReadOnly]
        private bool m_PauseSchedule;

        [ShowInInspector]
        [ReadOnly]
        private bool m_Roaming = false;

        [ShowInInspector]
        [ReadOnly]
        private int m_CurrentHangoutDuration = 0;

        [ShowInInspector]
        [ReadOnly]
        private Location m_CurrentLocation;

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();

            GEM.AddListener<HourChangedEvent>(CheckSchedule);
            GEM.AddListener<MinuteChangedEvent>(CheckSchedule);
        }

        private void Start()
        {
            InitializeSchedule();
        }

        #region Schedule Stuff

        public void InitializeSchedule()
        {
            for (var i = 0; i < Schedule.DailySchedule.Count; i++)
            {
                AddScheduleItem(Schedule.DailySchedule[i]);
            }
        }

        public void AddToSchedule(ScheduleItem scheduleItem, bool pauseSchedule = false)
        {
            m_PauseSchedule = pauseSchedule;
            AddScheduleItem(scheduleItem);
        }

        private void AddScheduleItem(ScheduleItem scheduleItem)
        {
            if (scheduleItem.Intervaled)
            {
                IntervaledActions.Add(scheduleItem.Interval, scheduleItem);
            }
            else
            {
                ScheduledActions.Add(scheduleItem.StartTime, scheduleItem);
            }
        }

        private void CheckSchedule(object arg)
        {
            if (m_PauseSchedule)
                return;

            TimeData currentTime = new TimeData();
            currentTime.Hour = TimeManager.Hour;
            currentTime.Minute = TimeManager.Minute;

            if (ScheduledActions.TryGetValue(currentTime, out var scheduleItem))
            {
                SetAction(scheduleItem);
            }

            if (m_Roaming)
            {
                m_CurrentHangoutDuration++;

                if (m_CurrentHangoutDuration == HANGOUT_DURATION)
                {
                    RoamAround();
                }
            }
        }

        #endregion

        #region Action Stuff

        public void SetLocationAction(NPCLocationActionType actionType, Location location, bool pauseSchedule = false)
        {
            m_PauseSchedule = pauseSchedule;
            DoLocationAction(actionType, location);
        }

        public void SetPlayerAction(NPCPlayerActionType type, bool pauseSchedule = false)
        {
            m_PauseSchedule = pauseSchedule;
            DoPlayerAction(type);
        }

        private void SetAction(ScheduleItem scheduleItem)
        {
            CurrentActionCategory = scheduleItem.ActionCategory;

            switch (CurrentActionCategory)
            {
                case ScheduledActionCategory.Location:
                    DoLocationAction(scheduleItem.NpcLocationActionType, scheduleItem.Location);
                    break;
                case ScheduledActionCategory.Player:
                    DoPlayerAction(scheduleItem.NpcPlayerActionType);
                    break;
            }
        }

        private void DoLocationAction(NPCLocationActionType type, Location location)
        {
            switch (type)
            {
                case NPCLocationActionType.RoamAround:
                    RoamAround();
                    break;
                case NPCLocationActionType.GoToLocation:
                    m_Roaming = false;
                    GoToLocation(location);
                    break;
                case NPCLocationActionType.HangoutAtLocation:
                    m_Roaming = false;
                    HangoutAtLocation(location);
                    break;
                case NPCLocationActionType.SpawnAtLocation:
                    SpawnAtLocation(location);
                    break;
            }
        }

        private void DoPlayerAction(NPCPlayerActionType type)
        {
            switch (type)
            {
                case NPCPlayerActionType.GoToPlayer:
                    GoToPlayer();
                    break;
                case NPCPlayerActionType.HangoutAtPlayer:
                    break;
                case NPCPlayerActionType.SpawnAtPlayer:
                    break;
                case NPCPlayerActionType.TalkToPlayer:
                    TalkToPlayer();
                    break;
            }
        }

        #endregion

        #region Location Stuff

        private void RoamAround()
        {
            m_Roaming = true;
            GoToLocation(NPCRouteManager.GetRandomLocation(), HangoutAtLocation);
        }

        private void GoToLocation(Location location, Action onComplete = null)
        {
            AnimationController.SetBool("Walking", true);
            Agent.SetDestination(NPCRouteManager.LocationToPosition[location]);

            Conditional.WaitFrames(5)
                .Do(() =>
                {
                    Conditional.If(() => Agent.HasReachedDestination()).Do(() =>
                    {
                        AnimationController.SetBool("Walking", false);
                        m_CurrentLocation = location;
                        onComplete?.Invoke();
                    });
                });
        }

        private void HangoutAtLocation()
        {
            Agent.isStopped = true;
            HangoutAtLocation(m_CurrentLocation);
        }

        private void HangoutAtLocation(Location location)
        {
            m_CurrentHangoutDuration = 0;
        }

        private void SpawnAtLocation(Location location)
        {
        }

        #endregion

        #region Player Stuff

        private void GoToPlayer(Action onComplete = null)
        {
            Agent.SetDestination(PlayerExtensions.GetPlayer().transform.position);

            Conditional.WaitFrames(5)
                .Do(() =>
                {
                    Conditional.If(() => Agent.HasReachedDestination()).Do(() =>
                    {
                        AnimationController.SetBool("Walking", false);
                        onComplete?.Invoke();
                    });
                });
        }

        private void TalkToPlayer()
        {
        }

        #endregion
    }
}