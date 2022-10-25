using Fate.PassiveSkills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fate.ShopKeeper.UI
{
    public class StatUI : MonoBehaviour
    {
        public StatType StatType;

        public TextMeshProUGUI StatName;
        public TextMeshProUGUI StatValue;

        public Image StatColorRefImg;

        public Color StatColor;

        public GameObject PlusSign;

        private int m_StatValue;

        public void SetStat(StatType type, int value, bool isIncreased = false)
        {
            StatType = type;

            var settings = FateSettings.Get();

            StatName.text = settings.StatNameShort[(int)type];
            StatColorRefImg.color = settings.StatColorRefs[(int)type];

            m_StatValue = value;
            StatValue.text = value.ToString();

            PlusSign.SetActive(isIncreased);
        }
        
        public void SetStat(string statName, string statValue, Color statColor, bool isIncreased = false)
        {
            StatName.text = statName;
            StatValue.text = statValue;
            StatColor = statColor;
            StatColorRefImg.color = StatColor;

            PlusSign.SetActive(isIncreased);
        }

        public void UpdateData(int increaseAmt)
        {
            if (increaseAmt > 0)
            {
                OnIncreaseStat(increaseAmt);
            }
            else
            {
                OnReset();
            }
        }

        private void OnIncreaseStat(int value)
        {
            StatValue.text = (m_StatValue + value).ToString();
            PlusSign.SetActive(true);
        }

        private void OnReset()
        {
            StatValue.text = m_StatValue.ToString();
            PlusSign.SetActive(false);
        }
    }
}