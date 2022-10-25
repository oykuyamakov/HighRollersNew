using System;
using System.Collections.Generic;
using Events;
using Sirenix.OdinInspector;

namespace TimeManagement
{
    public enum ScheduledActionCategory
    {
        None = 0,
        Location = 1,
        Player = 2,
    }

    public enum NPCLocationActionType
    {
        RoamAround,
        GoToLocation,
        HangoutAtLocation,
        SpawnAtLocation
    }

    public enum NPCPlayerActionType
    {
        GoToPlayer,
        HangoutAtPlayer,
        SpawnAtPlayer,
        TalkToPlayer
    }

    public enum Location
    {
        None = 0,
        AllAround = 1,
        Bar = 2,
        Market = 3,
        Bank = 4,
        Hub = 5,
        Shopkeeper = 6,
        CursedShopkeeper = 7,
        Sheriff = 8,
        Casino = 9,
        Church = 10,
        HorsePark = 11,
        CarDealer = 12,
        TrainStation = 13,
        Esnaf = 14,
        Brothel = 15,
        Cinema = 16,
        CoffinMaker = 17,
        EnergyStation = 18
    }

    [Serializable]
    public class ScheduleItem
    {
        public bool Intervaled;

        [ShowIf("@Intervaled")]
        [BoxGroup("Interval")]
        public TimeData Interval;

        [HideIf("@Intervaled")]
        [HorizontalGroup("Time", 0.5f, LabelWidth = 20)]
        [BoxGroup("Time/Start")]
        public TimeData StartTime;

        [HideIf("@Intervaled")]
        [BoxGroup("Time/End")]
        public TimeData EndTime;

        [ShowInInspector]
        public TimeData Length => StartTime - EndTime;

        [EnumToggleButtons]
        public ScheduledActionCategory ActionCategory;

        [ShowIf("ActionCategory", ScheduledActionCategory.Location)]
        public NPCLocationActionType NpcLocationActionType;

        [ShowIf("ActionCategory", ScheduledActionCategory.Player)]
        public NPCPlayerActionType NpcPlayerActionType;

        [ShowIf("ActionCategory", ScheduledActionCategory.Location)]
        [HideIf("NpcLocationActionType", NPCLocationActionType.RoamAround)]
        public Location Location;

        [ShowIf("NpcLocationActionType", NPCLocationActionType.RoamAround)]
        public List<Location> Locations;

        public Priority Priority = Priority.Normal;
    }

    [Serializable]
    public class SpecialScheduleItem : ScheduleItem
    {
    }

    [Serializable]
    public struct TimeData
    {
        [HorizontalGroup("Time", 0.5f, LabelWidth = 30)]
        [MinValue(0)]
        [MaxValue(23)]
        public int Hour;

        [HorizontalGroup("Time", 0.5f, LabelWidth = 30)]
        [MinValue(0)]
        [MaxValue(59)]
        public int Minute;

        public TimeData(int hour, int minute)
        {
            if (minute > 59)
            {
                var h = minute / 60;
                var m = minute % 60;

                hour += h;
                m += m;
            }

            if (hour > 23)
            {
                hour -= 23;
            }
            
            Hour = hour;
            Minute = minute;
        }

        public static TimeData operator +(TimeData a) => a;
        public static TimeData operator -(TimeData a) => new(-a.Hour, -a.Minute);
        public static TimeData operator +(TimeData a, TimeData b) => new TimeData(a.Hour + b.Hour, a.Minute + b.Minute);
        public static TimeData operator -(TimeData a, TimeData b) => a + -b;
    }
}