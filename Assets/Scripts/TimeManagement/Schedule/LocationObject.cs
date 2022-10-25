using UnityEngine;

namespace TimeManagement
{
    public class LocationObject : MonoBehaviour
    {
        [SerializeField] 
        private Location m_Location;
        
        public void Awake()
        {
            NPCRouteManager.LocationToPosition.Add(m_Location, transform.position);
        }
    }
}