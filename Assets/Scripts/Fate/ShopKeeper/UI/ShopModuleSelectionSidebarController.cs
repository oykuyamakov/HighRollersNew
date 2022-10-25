using System.Collections.Generic;
using Events;
using Fate.ShopKeeper.EventImplementations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Fate.ShopKeeper.UI
{
    public class ShopModuleSelectionSidebarController : MonoBehaviour
    {
        public List<ShopTabUI> ShopTabUis = new List<ShopTabUI>();

        [ShowInInspector]
        private ShopTabUI m_CurrentSelectedTab;

#if UNITY_EDITOR
        private void OnValidate()
        {
            ShopTabUis = new List<ShopTabUI>(GetComponentsInChildren<ShopTabUI>());
        }
#endif

        private void OnEnable()
        {
            GEM.AddListener<ShopSidebarTabSelectedEvent>(TabSelected);

            OnTabSelected(ShopTabUis[0]);
        }

        private void TabSelected(ShopSidebarTabSelectedEvent evt)
        {
            OnTabSelected(evt.ShopTabUI);
        }

        public void OnTabSelected(ShopTabUI shopTabUi)
        {
            if (m_CurrentSelectedTab != null)
            {
                m_CurrentSelectedTab.SetSelected(false);
            }

            m_CurrentSelectedTab = shopTabUi;
            m_CurrentSelectedTab.SetSelected(true);
        }
    }
}