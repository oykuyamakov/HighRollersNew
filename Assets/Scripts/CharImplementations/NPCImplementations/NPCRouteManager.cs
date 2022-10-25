using System.Collections.Generic;
using UnityCommon.Singletons;
using UnityEngine;

namespace TimeManagement
{
    public class NPCRouteManager : SingletonBehaviour<NPCRouteManager>
    {
        public static Dictionary<Location, Vector3> LocationToPosition = new Dictionary<Location, Vector3>();

        public static Location GetRandomLocation()
        {
            var rand = Random.Range(2, LocationToPosition.Count);
            return (Location)rand;
        }
        
        public static Vector3 GetRandomLocationPos()
        {
            var rand = Random.Range(2, LocationToPosition.Count);
            return LocationToPosition[(Location)rand];
        }
    }
}