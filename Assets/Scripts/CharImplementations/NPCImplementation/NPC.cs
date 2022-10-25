using System;
using System.Collections.Generic;
using AnimationManagement;
using CharImplementations;
using CharImplementations.NPCImplementation;
using Events;
using Sirenix.OdinInspector;
using TimeManagement;
using TimeManagement.EventImplementations;
using UnityCommon.Modules;
using UnityEngine;
using UnityEngine.AI;
using Utility.Extensions;

public class NPC : MonoBehaviour
{
    public const int HANGOUT_DURATION = 60;

    public NPCSchedule Schedule;

    public NavMeshAgent Agent;

    public AnimationController AnimationController;

    public Dictionary<TimeData, ScheduleItem> ScheduledActions = new Dictionary<TimeData, ScheduleItem>();

    // TODO: to be implemented
    public Dictionary<TimeData, ScheduleItem> IntervaledActions = new Dictionary<TimeData, ScheduleItem>();

    [ShowInInspector]
    private bool m_Roaming = false;

    [ShowInInspector]
    private int m_CurrentHangoutDuration = 0;

    [ShowInInspector]
    private Location m_CurrentLocation;
    
    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        GEM.AddListener<HourChangedEvent>(CheckSchedule);
        GEM.AddListener<MinuteChangedEvent>(CheckSchedule);
    }

    private void Start()
    {
        SetSchedule();
    }

    public void SetSchedule()
    {
        for (var i = 0; i < Schedule.DailySchedule.Count; i++)
        {
            if (Schedule.DailySchedule[i].Intervaled)
            {
                IntervaledActions.Add(Schedule.DailySchedule[i].Interval, Schedule.DailySchedule[i]);
            }
            else
            {
                ScheduledActions.Add(Schedule.DailySchedule[i].StartTime, Schedule.DailySchedule[i]);
            }
        }
    }

    private void CheckSchedule(object arg)
    {
        TimeData currentTime = new TimeData();
        currentTime.Hour = TimeManager.Hour;
        currentTime.Minute = TimeManager.Minute;

        if (ScheduledActions.TryGetValue(currentTime, out var scheduleItem))
        {
            switch (scheduleItem.ActionCategory)
            {
                case ScheduledActionCategory.Location:
                    DoLocationAction(scheduleItem.NpcLocationActionType, scheduleItem.Location);
                    break;
                case ScheduledActionCategory.Player:
                    DoPlayerAction(scheduleItem.NpcPlayerActionType);
                    break;
            }
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
        }
    }

    public void RoamAround()
    {
        m_Roaming = true;
        GoToLocation(NPCRouteManager.GetRandomLocation(), HangoutAtLocation);
    }

    public void GoToLocation(Location location, Action onComplete = null)
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

    public void HangoutAtLocation()
    {
        Agent.isStopped = true;
        HangoutAtLocation(m_CurrentLocation);
    }

    public void HangoutAtLocation(Location location)
    {
        m_CurrentHangoutDuration = 0;
    }

    public void GoToPlayer(Action onComplete = null)
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

    public void TalkToPlayer()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TalkToPlayer();
        }
    }

    public void SpawnAtLocation(Location location)
    {
    }
}