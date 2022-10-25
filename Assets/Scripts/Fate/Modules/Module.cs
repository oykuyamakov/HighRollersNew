using System;
using System.Collections.Generic;
using Fate.Modules.Data;

namespace Fate.Modules
{
    public abstract class Module
    {
        public virtual ModuleData Data { get; set; }
        
        //TODO: might be unnecessary
        public Dictionary<ModuleTier, int> OwnedModules = new();
        
        public abstract void OnActiveSkill(ModuleRuntimeData runtimeData);
        
        public virtual void AddToModules(ModuleTier tier)
        {
            if (OwnedModules.TryGetValue(tier, out int count))
            {
                OwnedModules[tier]++;
            }
            else
            {
                OwnedModules.Add(tier, 1);
            }
        }

        public virtual void RemoveFromModules(ModuleTier tier)
        {
            if (OwnedModules.TryGetValue(tier, out int count))
            {
                if(count == 0)
                    throw new Exception("your trippin man");
                
                OwnedModules[tier]--;
            }
            else
            {
                throw new Exception("your trippin man");
            }
        }
    }

    // public class Module<T> : Module
    // {
    //     
    // }
}

