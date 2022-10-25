using Fate.Modules.Data;
using UnityEngine;

namespace Fate.Modules
{
    public abstract class PlaceableModule : Module
    {
        protected PlaceableModuleData m_PlaceableModuleData => (PlaceableModuleData)Data;

        public abstract GameObject PlaceableObject { get; }
        public float MaxPlaceableDistance => ((PlaceableModuleData)Data).MaxPlaceableDistance;

        public abstract void OnPlaced();
    }
}