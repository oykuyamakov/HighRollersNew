using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using Fate.EventImplementations;
using Fate.Modules.Data;
using InventoryManagement;
using Pooling;
using Roro.Scripts.Utility;
using Sirenix.OdinInspector;
using UnityCommon.Modules;
using UnityCommon.Singletons;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Fate.Modules
{
    [DefaultExecutionOrder(ExecOrder.ModuleManager)]
    public class ModuleManager : SingletonBehaviour<ModuleManager>
    {
        private static int m_AvailableId = 0;
        
        [SerializeReference]
        public static List<Module> Modules = new List<Module>();

        public List<ModuleData> ModuleData = new List<ModuleData>();

        public InventorySystem ModuleInventorySystem;

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_AvailableId = 0;
            ModuleData.Clear();
            
            var moduleData = Resources.LoadAll<ModuleData>("Modules/");
            
            foreach (var data in moduleData)
            {
                data.Index = m_AvailableId++;
                ModuleData.Add(data);
                
                EditorUtility.SetDirty(data);
            }
        }
#endif

        private void Awake()
        {
            if (!SetupInstance())
                return;
        }

        private void OnEnable()
        {
            SetupModules();

            ModuleInventorySystem.Index = InventoryId.Module;

            GEM.AddListener<ToggleFateEditorUIEvent>(OnToggleFateEditor);

            GEM.AddListener<EquippedModulesModifiedEvent>(OnModuleSelectionModified);
            GEM.AddListener<AddModuleToInventoryEvent>(AddModule);
            GEM.AddListener<RemoveModuleFromInventoryEvent>(RemoveModule);
        }

        //TODO: temporary
        private void OnToggleFateEditor(ToggleFateEditorUIEvent evt)
        {
            ModuleInventorySystem.ToggleInventoryUI(evt.Visible);
        }

        public void SetupModules()
        {
            Modules = GetAllModules().ToList();

            for (var i = 0; i < Modules.Count; i++)
            {
                Modules[i].Data = ModuleData[i];
            }

            LoadModules();
        }

        private IEnumerable<Module> GetAllModules()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(Module)) && !type.IsAbstract)
                .Select(type => Activator.CreateInstance(type) as Module);
        }

        [Button]
        public void LoadModules()
        {
            var moduleCount = ModuleInventorySystem.Load();

            for (int i = 0; i < moduleCount; i++)
            {
                var moduleRuntimeData = new ModuleRuntimeData();
                moduleRuntimeData.Load(i);

                if (moduleRuntimeData.Equipped)
                {
                    FateManager.I.EquippedModules[moduleRuntimeData.Slot] = moduleRuntimeData;
                }

                var inventoryItem = new ModuleInventoryItem();
                inventoryItem.ModuleData = moduleRuntimeData;

                ModuleInventorySystem.AddToInventory(inventoryItem);
            }
            
            // TODO : i don't like this
            Conditional.WaitFrames(1)
                .Do(() =>
                {
                    ModuleInventorySystem.SendModifiedEvent();
                    using var evt = ModulesLoadedEvent.Get().SendGlobal();
                });
        }

        private void OnModuleSelectionModified(EquippedModulesModifiedEvent evt)
        {
            ModuleInventorySystem.SendModifiedEvent();
        }

        [Button]
        public void AddTestModules()
        {
            var tempList = ListPool<ModuleRuntimeData>.Get();

            for (var i = 0; i < Modules.Count; i++)
            {
                var module = Modules[i];
                var moduleRuntimeData = new ModuleRuntimeData();
                moduleRuntimeData.Module = module;
                moduleRuntimeData.Tier = ModuleTier.Legendary;
                moduleRuntimeData.Slot = -1;
                tempList.Add(moduleRuntimeData);
            }

            for (var i = 0; i < tempList.Count; i++)
            {
                var module = tempList[i];

                var inventoryItem = new ModuleInventoryItem();
                inventoryItem.ModuleData = module;

                ModuleInventorySystem.AddToInventory(inventoryItem);
            }

            ModuleInventorySystem.SendModifiedEvent();
        }

        public void AddModule(AddModuleToInventoryEvent evt)
        {
            var inventoryItem = new ModuleInventoryItem();
            inventoryItem.ModuleData = evt.ModuleRuntimeData;

            ModuleInventorySystem.AddToInventory(inventoryItem);
            ModuleInventorySystem.SendModifiedEvent();
        }

        public void RemoveModule(RemoveModuleFromInventoryEvent evt)
        {
            var item = ModuleInventorySystem.Items.Find(m =>
                (m as ModuleInventoryItem).ModuleData == evt.ModuleRuntimeData);

            ModuleInventorySystem.RemoveFromInventory(item);
        }

        public static ModuleTier GetTierForModule(int totalRolledVal)
        {
            return totalRolledVal switch
            {
                <= 4 => ModuleTier.Common,
                > 4 and <= 9 => ModuleTier.Uncommon,
                > 9 and <= 15 => ModuleTier.Rare,
                > 15 and <= 22 => ModuleTier.Epic,
                > 22 => ModuleTier.Legendary
            };
        }

        public static ModuleTier GetTierFromObject(object arg)
        {
            return arg is ModuleTier moduleTier ? moduleTier : ModuleTier.Common;
            ;
        }

        private void OnDisable()
        {
            GEM.RemoveListener<EquippedModulesModifiedEvent>(OnModuleSelectionModified);
            GEM.RemoveListener<AddModuleToInventoryEvent>(AddModule);
            GEM.RemoveListener<RemoveModuleFromInventoryEvent>(RemoveModule);
        }
    }
}