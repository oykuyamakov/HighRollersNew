using System.Collections.Generic;
using Sirenix.OdinInspector;
using TimeManagement;
using UnityEngine;

namespace CharImplementations.NPCImplementation
{
    [CreateAssetMenu(menuName = "Schedule/NPC")]
    public class NPCSchedule : ScriptableObject
    {
        [ListDrawerSettings]
        public List<ScheduleItem> DailySchedule = new List<ScheduleItem>();
    }
}