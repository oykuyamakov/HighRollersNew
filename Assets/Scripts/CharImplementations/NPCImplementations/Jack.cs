using CharImplementations.NPCImplementations;
using TimeManagement;
using UnityEngine;

namespace CharImplementations.NPCImplementation
{
    public class Jack : NPC
    {
        public NPCScheduleController ScheduleController;

        private void Start()
        {
            ScheduleController.SetLocationAction(NPCLocationActionType.HangoutAtLocation, Location.Church);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ScheduleController.SetPlayerAction(NPCPlayerActionType.TalkToPlayer);
            }
        }
    }
}