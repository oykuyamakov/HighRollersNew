using System;
using DG.Tweening;
using Fate.Modules;
using Promises;
using TMPro;
using UnityCommon.Runtime.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Utility.Extensions;

namespace Fate.ShopKeeper.UI
{
    public class ShopModuleCategoryTabUI : MonoBehaviour
    {
        public ModuleCategory Category;

        public Button SelectButton;

        public TextMeshProUGUI CategoryNameText;
        public Image BgImage;

        public Color SelectedColor;
        public Color UnselectedColor;

        //TODO: may do this differently
        private ModuleShopController m_ShopController;

        private bool m_Selected;
        private Promise<bool> m_SelectionPromise;

        private void OnEnable()
        {
            m_ShopController = GetComponentInParent<ModuleShopController>();
            SelectButton.onClick.AddListener(OnClick);
        }

        public void SetCategory(ModuleCategory category)
        {
            this.Category = category;
            CategoryNameText.text = category.ToString();
        }

        public void OnClick()
        {
            transform.DoScaleAnim(1.1f, 0.1f);

            if (m_Selected)
                return;

            m_Selected = true;
            OnSelected();
        }

        private void OnSelected()
        {
            m_SelectionPromise = m_ShopController.SetCategory(Category);

            m_SelectionPromise.OnResultT += (b, b1) =>
            {
                OnDeselected();
            };
            
            BgImage.DOColor(SelectedColor, 0.1f);
            CategoryNameText.color = CategoryNameText.color.WithAlpha(1);
        }
        
        private void OnDeselected()
        {
            m_Selected = false;
            BgImage.DOColor(UnselectedColor, 0.1f);
            CategoryNameText.color = CategoryNameText.color.WithAlpha(0.5f);
        }

        private void ToggleSelection(bool selected)
        {
            if (selected)
            {
                OnSelected();
            }
            else
            {
                OnDeselected();
            }
        }

        private void OnDisable()
        {
            SelectButton.onClick.RemoveListener(OnClick);
        }
    }
}