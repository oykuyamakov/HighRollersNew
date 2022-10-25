using UnityEngine;

namespace Fate.Modules
{
    public class NotABrickWall : PlaceableModule
    {
        private NotABrickWallBehaviour m_WallInstance;
        public override GameObject PlaceableObject => m_WallInstance.gameObject;
        
        public override void OnActiveSkill(ModuleRuntimeData runtimeData)
        {
            m_WallInstance = Object.Instantiate(m_PlaceableModuleData.PlaceableObjectPrefab).GetComponent<NotABrickWallBehaviour>();
            m_WallInstance.transform.localScale *= m_PlaceableModuleData.Scale.x;

            var duration = runtimeData.GetDurationData();
            var health = runtimeData.GetData(1);
            
            m_WallInstance.Setup(health, duration);
        }

        public override void OnPlaced()
        {
            m_WallInstance.StartTimer();
        }
    }
}