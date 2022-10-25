using UnityCommon.Runtime.Extensions;
using UnityEngine;

namespace Fate.Modules
{
    public class PlaceableBehaviour : MonoBehaviour
    {
        public bool Placeable => !(m_Colliding || m_LeftGround);

        private Renderer m_Renderer;
        private Collider m_Collider;

        private Material m_OriginalMaterial;

        private bool m_Colliding;
        private bool m_LeftGround;

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
                m_LeftGround = false;
                return;
            }

            m_Colliding = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Level"))
            {
                m_LeftGround = true;
                return;
            }

            m_Colliding = false;
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