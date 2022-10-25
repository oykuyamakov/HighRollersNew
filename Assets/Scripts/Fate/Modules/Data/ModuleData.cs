using System.Collections.Generic;
using Fate.PassiveSkills;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Fate.Modules.Data
{
    [CreateAssetMenu(menuName = "Fate/Modules/Data")]
    public class ModuleData : ScriptableObject
    {
        public int Index;

        [BoxGroup("Data shown to player")]
        public string ModuleName;

        [BoxGroup("Data shown to player")]
        public string ModuleDescription;

        [BoxGroup("Data shown to player")]
        public Sprite ModuleIcon;

        [BoxGroup("Attributes")]
        public List<ModuleParameter> Parameters = new List<ModuleParameter>();

        [SerializeReference]
        [BoxGroup("Attributes")]
        public List<PassiveSkill> PassiveSkills = new List<PassiveSkill>();
    }
}