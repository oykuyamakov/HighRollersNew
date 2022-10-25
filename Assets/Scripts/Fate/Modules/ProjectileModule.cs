using CombatManagement.ProjectileManagement;
using Fate.Modules.Data;
using UnityEngine;

namespace Fate.Modules
{
    public abstract class ProjectileModule : Module
    {
        protected PlaceableModuleData m_PlaceableModuleData => (PlaceableModuleData)Data;
        
        public ProjectileType ProjectileType;
        
        public float MaxPlaceableDistance => m_PlaceableModuleData.MaxPlaceableDistance;
        
        public float ProjectileSpeed => m_PlaceableModuleData.ProjectileSpeed;
        
        public abstract void OnThrown(Transform indicator);
    }
}