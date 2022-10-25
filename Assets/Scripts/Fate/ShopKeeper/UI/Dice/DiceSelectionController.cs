using DG.Tweening;
using Fate.Modules.Data;
using Fate.ShopKeeper.UI;
using Promises;
using TMPro;
using UnityCommon.Modules;
using UnityCommon.Runtime.Extensions;
using UnityCommon.Variables;
using UnityEngine;
using UnityEngine.UI;
using Utility.Extensions;
using ModuleManager = Fate.Modules.ModuleManager;

namespace Fate.ShopKeeper
{
    public class DiceSelectionController : MonoBehaviour
    {
        public Canvas Canvas;

        [SerializeField]
        private IntVariable m_Dice;

        public static int MaxCount = 7;

        public CanvasGroup SelectionGroup;
        public CanvasGroup ResultGroup;

        [Header("Dice Selection Stuff")]
        public ModuleTitleUI ModuleTitleUI;
        public Slider DiceSelectionSlider;
        public TextMeshProUGUI CurrentDiceCountText;

        public TextMeshProUGUI[] StatisticsTexts = new TextMeshProUGUI[5];
        public string[] Statistics = new string[5];

        public Button SubmitButton;
        public Button CancelButton;
        
        [Header("Roll Result Stuff")]
        public TextMeshProUGUI TotalRolledText;
        public TextMeshProUGUI TotalRolledCommentText;

        public TextMeshProUGUI RarityCheckText;
        public Image RarityCheckBg;

        public Transform RollButtonT;
        public Transform RarityCheckT;

        [SerializeField]
        private DiceRollAnimationController m_DiceAnimationController;

        private int m_SelectedDiceCount = 1;

        private int m_TotalRolled;

        private Promise<int> m_Promise;

        private void OnEnable()
        {
            DiceSelectionSlider.onValueChanged.AddListener(OnSliderValueChanged);
            SubmitButton.onClick.AddListener(Submit);
            CancelButton.onClick.AddListener(Cancel);

            Close();
        }

        public Promise<int> Instantiate(ModuleData moduleData)
        {
            m_Promise = Promise<int>.Create();

            ModuleTitleUI.SetModuleInfo(moduleData);
            UpdateStatistics();
            
            Open();

            return m_Promise;
        }
        
        private void AddDice()
        {
            if (m_SelectedDiceCount >= m_Dice.Value)
            {
                // TODO: some effect
                return;
            }

            m_SelectedDiceCount++;
            m_DiceAnimationController.AddDice();
        }

        private void RemoveDice()
        {
            if (m_SelectedDiceCount <= 1)
            {
                // TODO: some effect
                return;
            }

            m_SelectedDiceCount--;
            m_DiceAnimationController.RemoveDice();
        }

        private void OnSliderValueChanged(float sliderVal)
        {
            if (sliderVal > m_SelectedDiceCount)
            {
                AddDice();
            }
            else
            {
                RemoveDice();
            }

            CurrentDiceCountText.text = m_SelectedDiceCount.ToString();
        }

        private void UpdateStatistics()
        {
            for (var i = 0; i < StatisticsTexts.Length; i++)
            {
                StatisticsTexts[i].text = Statistics[i];
            }
        }
        
        private void Submit()
        {
            SelectionGroup.Toggle(false, 0.1f);
            ResultGroup.Toggle(true, 0.1f);

            var diceRolled = m_DiceAnimationController.RollDiceAnim();

            diceRolled.OnResultT += (success, totalRolled) =>
            {
                if(!success)
                    return;
                
                OnDiceRollAnimationComplete(totalRolled);
            };
        }

        private void OnDiceRollAnimationComplete(int totalRolled)
        {
            TotalRolledText.text = $"{totalRolled} out of {m_SelectedDiceCount}";
            TotalRolledCommentText.text =
                totalRolled == m_SelectedDiceCount ? "Perfect!" : "Not bad!"; // aynen kanka

            var rarity = ModuleManager.GetTierForModule(totalRolled);
            RarityCheckText.text = $"{rarity} QUALITY CHECK";
            RarityCheckBg.color = FateSettings.Get().RarityColorRefs[(int)rarity];

            RarityCheckT.DOScaleX(1f, 0.35f).SetEase(Ease.OutBack);

            Conditional.Wait(1.35f)
                .Do(() =>
                {
                    m_Promise.Complete(m_TotalRolled);
                    Close();
                });
        }

        private void Cancel()
        {
            Conditional.WaitFrames(1)
                .Do(() => m_Promise.Fail());

            Close();
        }

        private void Open()
        {
            Canvas.enabled = true;
            SelectionGroup.Toggle(true, 0.1f);
        }

        private void Close()
        {
            Canvas.enabled = false;
            m_SelectedDiceCount = 1;
            DiceSelectionSlider.value = 1;
            
            ResultGroup.Toggle(false, 0f);
            
            TotalRolledText.text = "";
            TotalRolledCommentText.text = "";
            RarityCheckT.localScale = RarityCheckT.localScale.WithX(0);
        }

        private void OnDisable()
        {
            DiceSelectionSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
            SubmitButton.onClick.RemoveListener(Submit);
            CancelButton.onClick.RemoveListener(Cancel);
        }
    }
}