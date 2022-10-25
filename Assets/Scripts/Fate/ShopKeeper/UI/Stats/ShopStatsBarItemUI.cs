using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Utility;

namespace Fate.ShopKeeper.UI.Stats
{
    public class ShopStatsBarItemUI : MonoBehaviour
    {
        public List<TextMeshProUGUI> StatTexts = new List<TextMeshProUGUI>();

#if UNITY_EDITOR
        private void OnValidate()
        {
            StatTexts = new List<TextMeshProUGUI>(GetComponentsInChildren<TextMeshProUGUI>());
        }
#endif

        public void Setup(List<ModuleStatPair> pairs)
        {
            StringBuilder stringBuilder = StringBuilderPool.Get();

            for (var i = 0; i < StatTexts.Count; i++)
            {
                stringBuilder.Clear();

                var pair = pairs[i];
                StatTexts[i].text = stringBuilder.Append(pair.Module.Data.ModuleName).Append("+").Append(pair.Value)
                    .ToString();
            }
        }
    }
}