using Promises;
using TMPro;
using UnityCommon.Modules;
using UnityCommon.Variables;
using UnityEngine;
using UnityEngine.UI;
using Utility.Extensions;

namespace Fate.ShopKeeper
{
    public class DiceSelectionPopup : MonoBehaviour
    {
        [SerializeField]
        private IntVariable m_Dice;

        public TextMeshProUGUI DiceCountText;

        public Button AddButton;
        public Button RemoveButton;

        public Button SubmitButton;
        public Button CancelButton;

        public CanvasGroup CanvasGroup;

        [SerializeField]
        private DiceRollAnimationController m_DiceAnimationController;
        
        private int m_SelectedDiceCount = 1;

        private Promise<int> m_Promise;

        private void OnEnable()
        {
            AddButton.onClick.AddListener(AddDice);
            RemoveButton.onClick.AddListener(RemoveDice);
            SubmitButton.onClick.AddListener(Submit);
            CancelButton.onClick.AddListener(Cancel);
            
            Close();
        }

        public Promise<int> Instantiate()
        {
            DiceCountText.text = m_SelectedDiceCount.ToString();
            m_Promise = Promise<int>.Create();
            
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
            
            DiceCountText.text = (++m_SelectedDiceCount).ToString();
           // m_DiceAnimationController.AddDice();
        }

        private void RemoveDice()
        {
            if (m_SelectedDiceCount <= 1)
            {
                // TODO: some effect
                return;
            }

            DiceCountText.text = (--m_SelectedDiceCount).ToString();
          //  m_DiceAnimationController.RemoveDice();
        }

        private void Submit()
        {
            var totalRolledVal = 0;
            
            for (var i = 0; i < m_SelectedDiceCount/*m_DiceAnimationController.DiceObjects.Count*/; i++)
            {
                // totalRolledVal += m_DiceAnimationController.DiceObjects[i].RollDice(Dice.RollADie());
                totalRolledVal += Dice.RollADie();
            }
            
            Conditional.WaitFrames(1)
                .Do(() =>
                {
                    m_Promise.Complete(totalRolledVal);
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
            CanvasGroup.Toggle(true, 0.25f);
        }

        private void Close()
        {
            CanvasGroup.Toggle(false, 0);
            m_SelectedDiceCount = 1;
        }
        
        private void OnDisable()
        {
            AddButton.onClick.RemoveListener(AddDice);
            RemoveButton.onClick.RemoveListener(RemoveDice);
            SubmitButton.onClick.RemoveListener(Submit);
            CancelButton.onClick.RemoveListener(Cancel);
        }
    }
}