using DG.Tweening;
using Events;
using Fate.ShopKeeper.EventImplementations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility.Extensions;

namespace Fate.ShopKeeper.UI
{
    public class ShopTabUI : MonoBehaviour
    {
        public CanvasGroup TabCanvasGroup;

        public Button SelectButton;

        public Image TabImage;
        public TextMeshProUGUI TabText;

        public Color SelectedTextColor;
        public Color UnselectedTextColor;

        private bool m_IsSelected;

        private void OnEnable()
        {
            SelectButton.onClick.AddListener(OnClick);
        }

        public void OnClick()
        {
            if (m_IsSelected)
                return;

            SetSelected(true);

            using var evt = ShopSidebarTabSelectedEvent.Get(this).SendGlobal();
        }

        public void SetSelected(bool isSelected)
        {
            if (m_IsSelected == isSelected)
                return;

            m_IsSelected = isSelected;
            TabImage.DOColor(isSelected ? Color.white : Color.clear, 0.1f);
            TabText.DOColor(isSelected ? SelectedTextColor : UnselectedTextColor, 0.1f);

            if (TabCanvasGroup != null)
                TabCanvasGroup.Toggle(isSelected, 0.1f);
        }

        private void OnDisable()
        {
            SelectButton.onClick.RemoveListener(OnClick);
        }
    }
}