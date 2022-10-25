using System.Collections.Generic;
using System.Linq;
using CombatManagement.EventImplementations;
using Events;
using Fate.EventImplementations;
using Fate.Modules;
using Fate.Modules.Data;
using Fate.PassiveSkills;
using Fate.ShopKeeper.UI.Stats;
using InventoryManagement;
using Pooling;
using Roro.Scripts.Utility;
using Sirenix.OdinInspector;
using UnityCommon.Modules;
using UnityCommon.Runtime.Extensions;
using UnityCommon.Singletons;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Fate
{
    [DefaultExecutionOrder(ExecOrder.ModuleManager + 1)]
    public class FateManager : SingletonBehaviour<FateManager>
    {
        private const int FateSlotCount = 6;
        private const int CursedModuleSlot = 0;

        [SerializeReference]
        public ModuleRuntimeData[] EquippedModules = new ModuleRuntimeData[FateSlotCount];

        public ModuleRuntimeData CurrentRolledModule;

        public InventorySystem FateInventorySystem;

        public static bool FateAttackInProgress;

        // TODO: STATS SECTION TO BE REFACTORED
        public static List<int> GetEquippedModuleStats()
        {
            var stats = ListPool<int>.Get();

            var vitality = 0f;
            var speed = 0f;
            var damage = 0f;

            if (!I.IsFateEmpty())
            {
                var tuple = I.IterateOverModuleStats();

                vitality += tuple.Item1;
                speed += tuple.Item2;
                damage += tuple.Item3;
            }

            stats.Add((int)vitality);
            stats.Add((int)speed);
            stats.Add((int)damage);

            return stats;
        }

        public static Dictionary<StatType, List<ModuleStatPair>> GetEquippedModuleStatPairs()
        {
            Dictionary<StatType, List<ModuleStatPair>> stats = new Dictionary<StatType, List<ModuleStatPair>>();

            for (var i = 0; i < I.EquippedModules.Length; i++)
            {
                var module = I.EquippedModules[i];

                if (module == null)
                    continue;

                foreach (var stat in module.Stats)
                {
                    var pair = new ModuleStatPair(module.Module, stat.Value);

                    if (stats.ContainsKey(stat.Key))
                        stats[stat.Key].Add(pair);
                    else
                        stats.Add(stat.Key, new List<ModuleStatPair> { pair });
                }
            }

            return stats;
        }

        // TODO: tuple makes it hard-coded.
        private (int, int, int) IterateOverModuleStats()
        {
            var tuple = (0, 0, 0);

            for (var i = 0; i < I.EquippedModules.Length; i++)
            {
                var module = I.EquippedModules[i];

                if (module == null)
                    continue;

                var moduleStats = module.GetModuleStats();

                tuple.Item1 += moduleStats.Item1;
                tuple.Item2 += moduleStats.Item2;
                tuple.Item3 += moduleStats.Item3;
            }

            return tuple;
        }
        
        private void Awake()
        {
            if (!SetupInstance())
                return;
        }

        private void OnEnable()
        {
            GEM.AddListener<ModulesLoadedEvent>(OnModulesLoaded);

            GEM.AddListener<ActivatePassiveEffectsEvent>(OnActivatePassiveEffects);
            GEM.AddListener<FateEnergyFullEvent>(OnFateReadyToAttack, Priority.Critical);
            GEM.AddListener<CallFateAttackEvent>(OnFateAttackCalled);

            GEM.AddListener<ModuleEquipmentEvent>(OnModuleSelectedEvent);

            FateInventorySystem.Index = InventoryId.Fate;
        }

        private void OnModulesLoaded(ModulesLoadedEvent evt)
        {
            UpdateInventorySystem();
        }

        [Button]
        private void OnActivatePassiveEffects(ActivatePassiveEffectsEvent evt)
        {
            for (var i = 0; i < EquippedModules.Length; i++)
            {
                EquippedModules[i].OnPassiveSkill(null);
            }
        }

        /// <summary>
        /// randomize and determine activated module.
        /// if cursed, it will automatically activate its attack
        /// </summary>
        private void OnFateReadyToAttack(FateEnergyFullEvent evt)
        {
            if (EquippedModules.All(m => m == null))
                return;

            CurrentRolledModule = EquippedModules[GetRandomSlot()];

            if (CurrentRolledModule.IsCursed)
            {
                CursedModuleRolled();
            }

            evt.RolledModule = CurrentRolledModule;
        }

        private void CursedModuleRolled()
        {
            var cursedData = CurrentRolledModule.Data as CursedModuleData;

            // TODO: maybe get this from the module and don't calculate here
            var waitTime = cursedData.ActivationWaitTimes.GetParameter(CurrentRolledModule.Tier);

            Conditional.Wait(waitTime)
                .Do(() => { OnFateAttackCalled(null); });
        }

        /// <summary>
        /// activate attack for CurrentRolledModule
        /// </summary>
        [Button]
        private void OnFateAttackCalled(CallFateAttackEvent evt)
        {
            if (FateAttackInProgress)
            {
                CancelFateAttack();
                return;
            }
            
            if (!FateEnergyManager.IsFateEnergyFull)
                return;
            
            FateAttackInProgress = true;

            using var activatedEvent = FateAttackActivatedEvent.Get(CurrentRolledModule).SendGlobal();

            if (CurrentRolledModule.IsProjectile)
            {
                OnProjectileModuleAttack();
                return;
            }

            if (CurrentRolledModule.IsPlaceable)
            {
                OnPlaceableModuleAttack();
                return;
            }

            CurrentRolledModule.OnActiveSkill(null);
            ConcludeFateAttack();
        }

        private void CancelFateAttack()
        {
            FateAttackInProgress = false;
            using var evt = Event<CancelFateAttackEvent>.Get().SendGlobal();
        }

        // TODO: slow down time, activate playerprojectilethrower and throw the projectile
        // then activate the module's attack. set fate attack in progress false.
        private void OnProjectileModuleAttack()
        {
            var projectileModule = CurrentRolledModule.Module as ProjectileModule;

            CurrentRolledModule.OnActiveSkill(null);

            using (var evt = ActivatePlayerProjectileThrowerEvent.Get(projectileModule.MaxPlaceableDistance,
                       projectileModule.ProjectileType))
            {
                evt.SendGlobal();

                var promise = evt.ProjectilePromise;
                promise.OnResultT += ((b, direction) =>
                {
                    projectileModule.OnThrown(direction);

                    Conditional.Wait(1)
                        .Do(ConcludeFateAttack);

                    promise.Release();
                });
            }
        }

        private void OnPlaceableModuleAttack()
        {
            var placeableModule = CurrentRolledModule.Module as PlaceableModule;

            CurrentRolledModule.OnActiveSkill(null);

            using (var evt = ActivatePlayerObjectPlacerEvent.Get(placeableModule.MaxPlaceableDistance,
                       placeableModule.PlaceableObject))
            {
                evt.SendGlobal();

                var promise = evt.PlacedPromise;
                promise.OnResultT += ((b, b1) =>
                {
                    placeableModule.OnPlaced();

                    Conditional.Wait(1)
                        .Do(ConcludeFateAttack);

                    promise.Release();
                });
            }
        }

        /// <summary>
        /// basically called after ActiveSkill has been activated for the current module
        /// after the attack has been called & if there was a placeable or projectile module they have been set,
        /// call this to restart filling the energy & stuff.
        /// </summary>
        private void ConcludeFateAttack()
        {
            using var fateAttackCalledEvt = ConcludeFateAttackEvent.Get().SendGlobal();
            FateAttackInProgress = false;
        }

        /// <summary>
        /// equip if unequipped and try equip if unequipped
        /// </summary>
        /// <param name="evt"></param>
        private void OnModuleSelectedEvent(ModuleEquipmentEvent evt)
        {
            var equipped = evt.SelectedModule.Equipped;

            if (equipped)
            {
                OnModuleUnequipped(evt.SelectedModule);
            }
            else
            {
                OnModuleEquipped(evt.SelectedModule);
            }

            OnEquippedModulesEdited();

            using var modifiedEvt = EquippedModulesModifiedEvent.Get().SendGlobal();
        }

        /// <summary>
        /// put the module on the first available empty spot (if there is one) or the selected slot if acceptable (not implemented yet)
        /// first slot (index : 0) always belongs to a cursed module
        /// </summary>
        private bool OnModuleEquipped(ModuleRuntimeData module)
        {
            if (module.IsCursed)
            {
                return EquipCursedModule(module);
            }

            if (!CanAddToFate())
            {
                OnEquipRejected();
                return false;
            }

            EquipModule(module);
            return true;
        }

        private bool EquipCursedModule(ModuleRuntimeData module)
        {
            if (EquippedModules[CursedModuleSlot] != null)
            {
                EquippedModules[CursedModuleSlot].Slot = -1;
            }

            EquippedModules[CursedModuleSlot] = module;
            module.Slot = CursedModuleSlot;

            return true;
        }

        private void EquipModule(ModuleRuntimeData module)
        {
            var slot = GetNextEmptySlot();
            EquippedModules[slot] = module;
            module.Slot = slot;
        }

        private void OnEquipRejected()
        {
            // TODO: shake stuff
            Debug.Log("you need to remove a module before being able to add one");
        }

        private void OnModuleUnequipped(ModuleRuntimeData module)
        {
            EquippedModules[module.Slot] = null;
            module.Slot = -1;
        }

        // TODO: must have at least one cursed to be accepted
        private bool OnEquippedModulesEdited()
        {
            UpdateInventorySystem();

            return EquippedModules[CursedModuleSlot] != null && EquippedModules[CursedModuleSlot].IsCursed;
        }

        private void UpdateInventorySystem()
        {
            FateInventorySystem.ClearInventory();

            for (var i = 0; i < EquippedModules.Length; i++)
            {
                var module = EquippedModules[i];

                if (module == null)
                    continue;

                // TODO : don't create new every time
                var inventoryItem = new ModuleInventoryItem();
                inventoryItem.ModuleData = module;

                FateInventorySystem.AddToInventory(inventoryItem);
            }

            FateInventorySystem.SendModifiedEvent();
        }

        private int GetRandomSlot()
        {
            var slot = Random.Range(0, FateSlotCount);

            if (EquippedModules[slot] == null)
                return GetRandomSlot();

            return slot;
        }

        private bool CanAddToFate()
        {
            return GetNextEmptySlot() > 0;
        }

        private int GetNextEmptySlot()
        {
            return EquippedModules.FirstIndexOf(m => m == null, 1);
        }

        private bool IsFateEmpty() => EquippedModules.All(m => m == null);

        private void OnDisable()
        {
            GEM.RemoveListener<ModulesLoadedEvent>(OnModulesLoaded);

            GEM.RemoveListener<ActivatePassiveEffectsEvent>(OnActivatePassiveEffects);
            GEM.RemoveListener<FateEnergyFullEvent>(OnFateReadyToAttack);
            GEM.RemoveListener<CallFateAttackEvent>(OnFateAttackCalled);

            GEM.RemoveListener<ModuleEquipmentEvent>(OnModuleSelectedEvent);
        }
    }
}