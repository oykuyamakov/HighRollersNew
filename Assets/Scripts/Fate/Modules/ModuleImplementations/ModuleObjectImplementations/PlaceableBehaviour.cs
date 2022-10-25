using System;
using UnityCommon.Runtime.Extensions;
using UnityEngine;

namespace Fate.Modules
{
    public class PlaceableBehaviour : MonoBehaviour
    {
        public bool Placeable;

        private Renderer m_Renderer;
        private Collider m_Collider;

        private Material m_OriginalMaterial;

        private void Awake()
        {
            m_Renderer = GetComponent<Renderer>();
            m_Collider = GetComponent<Collider>();

            m_OriginalMaterial = m_Renderer.material;

            m_Collider.isTrigger = true;
            gameObject.SetLayer(LayerMask.NameToLayer("PlaceableObject"));
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Level"))
            {
                Placeable = true;
                return;
            }
            
            Placeable = false;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Level"))
            {
                Placeable = false;
                return;
            }
            
            Placeable = true;
        }

        public void AssignMaterial(Material m)
        {
            m_Renderer.material = m;
        }

        public void Reset()
        {
            m_Collider.isTrigger = false;
            gameObject.SetLayer(LayerMask.NameToLayer("Obstacle"));
            m_Renderer.material = m_OriginalMaterial;
            Destroy(this);
        }
    }
}