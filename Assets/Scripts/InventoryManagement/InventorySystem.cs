using System;
using System.Collections.Generic;
using Events;
using InventoryManagement.EventImplementations;
using Roro.Scripts.Serialization;
using Sirenix.OdinInspector;
using UnityEngine;

namespace InventoryManagement
{
    /// <summary>
    /// simply holds a list of inventory items with save/load functionality
    /// </summary>
    public class InventorySystem : MonoBehaviour
    {
        // TODO: maybe?
        // private static int m_AvailableId = 0;

        public int Index;

        [SerializeReference]
        public List<InventoryItem> Items = new List<InventoryItem>();

        private SerializationWizard m_SerializationWizard = SerializationWizard.Default;

// #if UNITY_EDITOR
//         private void OnValidate()
//         {
//             Index = m_AvailableId++;
//         }
// #endif

        [Button]
        public void AddToInventory(InventoryItem item)
        {
            Items.Add(item);
        }

        [Button]
        public void RemoveFromInventory(InventoryItem item)
        {
            if (!Items.Contains(item))
            {
                throw new Exception("your trippin man");
            }
            else
            {
                Items.Remove(item);
            }
        }

        public void ClearInventory()
        {
            Items.Clear();
            SendModifiedEvent();
        }

        public void ToggleInventoryUI(bool visibility)
        {
            using (var evt = ToggleInventoryUIEvent.Get(visibility))
            {
                this.SendEvent(evt);
            }
        }

        /// <summary>
        /// if there is a ui controller listening, it will update the ui
        /// </summary>
        public void SendModifiedEvent()
        {
            using (var evt = InventoryModifiedEvent.Get())
            {
                this.SendEvent(evt);
            }
        }

        private void OnDisable()
        {
            Save();
        }

        public int Load()
        {
            return m_SerializationWizard.GetInt($"inventory_{Index}");
        }

        public void Save()
        {
            m_SerializationWizard.SetInt($"inventory_{Index}", Items.Count);

            for (var i = 0; i < Items.Count; i++)
            {
                Items[i].Save(i);
            }
        }
    }
}