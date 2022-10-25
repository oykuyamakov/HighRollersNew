using System.Collections.Generic;
using Events;
using InventoryManagement.EventImplementations;
using Rewired;
using Rewired.Integration.UnityUI;
using Sirenix.OdinInspector;
using UnityCommon.Runtime.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Utility.Extensions;

namespace InventoryManagement.UI
{
    /// <summary>
    /// ui controller for an inventory system
    /// must assign the inventory system :) maybe i will find something better here
    /// </summary>
    public class InventoryUIController : MonoBehaviour
    {
        public const int InventoryUiPoolSize = 10;

        public InventorySystem InventorySystem;

        public InventoryItemUI InventoryItemUIPrefab;

        public Transform ContentArea;

        public CanvasGroup CanvasGroup;

        [ShowInInspector]
        private List<InventoryItemUI> m_ItemUis = new List<InventoryItemUI>();

        private void OnEnable()
        {
            InventorySystem.AddListener<InventoryModifiedEvent>(SetupUI);
            InventorySystem.AddListener<ToggleInventoryUIEvent>(OnToggleUI);

            Initialize();
        }

        private void Initialize()
        {
            for (var i = 0; i < InventoryUiPoolSize; i++)
            {
                var itemUI = Instantiate(InventoryItemUIPrefab, ContentArea);
                itemUI.gameObject.SetActive(false);
                m_ItemUis.Add(itemUI);
            }
        }

        [Button]
        public void SetupUI(object arg)
        {
            int itemCount = InventorySystem.Items.Count;
            bool largerThanPoolSize = itemCount > InventoryUiPoolSize;

            for (var i = 0; i < itemCount; i++)
            {
                var item = InventorySystem.Items[i];

                var itemUI = i >= m_ItemUis.Count ? Instantiate(InventoryItemUIPrefab, ContentArea) : m_ItemUis[i];
                itemUI.InventoryItem = item;
                itemUI.UpdateUI(item.GetData());

                m_ItemUis.AddDistinct(itemUI);

                itemUI.gameObject.SetActive(true);
            }

            if (largerThanPoolSize)
                return;

            for (var k = itemCount; k < m_ItemUis.Count; k++)
            {
                m_ItemUis[k].gameObject.SetActive(false);
            }
        }

        public void ClearUI()
        {
            for (var i = 0; i < m_ItemUis.Count; i++)
            {
                m_ItemUis[i].gameObject.SetActive(false);
            }
        }

        public void OnToggleUI(ToggleInventoryUIEvent evt)
        {
            if(CanvasGroup == null)
                return;
            
            CanvasGroup.Toggle(evt.Visible, 0.45f);

            if (!evt.Visible)
                return;

            // RewiredEventSystem.current.SetSelectedGameObject(m_ItemUis[0].gameObject.GetComponentInChildren<Button>()
            //     .gameObject);
            //
            // foreach (var mapEnablerRuleSet in ReInput.players.GetPlayer(0).controllers.maps.mapEnabler.ruleSets)
            // {
            //     Debug.Log(mapEnablerRuleSet.tag);
            //     
            //     if (mapEnablerRuleSet.tag == "ui")
            //     {
            //         mapEnablerRuleSet.enabled = true;
            //     }
            //     else
            //     {
            //         mapEnablerRuleSet.enabled = false;
            //     }
            // }
            //
            // ReInput.players.GetPlayer(0).controllers.maps.mapEnabler.Apply();         
        }

        private void OnDisable()
        {
            InventorySystem.RemoveListener<InventoryModifiedEvent>(SetupUI);
            InventorySystem.RemoveListener<ToggleInventoryUIEvent>(OnToggleUI);
        }
    }
}