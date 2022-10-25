using UnityEngine;

namespace Fate.Modules
{
    public class PlaceableModuleBase : MonoBehaviour
    {
        public virtual void OnContact(Transform contactT)
        {
            
        }
        
        public virtual void OnContact(float damage)
        {
            
        }
    }
}