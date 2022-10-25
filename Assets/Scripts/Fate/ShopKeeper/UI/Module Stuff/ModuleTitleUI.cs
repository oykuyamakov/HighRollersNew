using Fate.Modules.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fate.ShopKeeper.UI
{
    public class ModuleTitleUI : MonoBehaviour
    {
        public Image ModuleIcon;
        public TextMeshProUGUI ModuleNameText;

        public void SetModuleInfo(ModuleData moduleData)
        {
            ModuleIcon.sprite = moduleData.ModuleIcon;
            ModuleNameText.text = moduleData.ModuleName;
        }
    }
}