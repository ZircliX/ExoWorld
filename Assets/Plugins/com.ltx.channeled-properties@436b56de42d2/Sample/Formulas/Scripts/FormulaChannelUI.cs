using System;
using LTX.ChanneledProperties.Formulas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LTX.ChanneledProperties.Samples.Sample.Formulas.Scripts
{
    public class FormulaChannelUI : MonoBehaviour
    {
        private FormulaUI formulaUI;
        private FormulaGroupUI groupUI;

        [SerializeField]
        private TMP_Text text;

        [SerializeField]
        private Slider valuesSlider;
        [SerializeField]
        private TMP_Dropdown operatorDropdown;
        [SerializeField]
        private TMP_Dropdown multiplyByDropdown;

        [SerializeField]
        private CanvasGroup canvasGroup;

        private void Start()
        {
            formulaUI = GetComponentInParent<FormulaUI>();
            groupUI = GetComponentInParent<FormulaGroupUI>();
            valuesSlider.onValueChanged.RemoveAllListeners();
            valuesSlider.onValueChanged.AddListener((value) => Sync());

            operatorDropdown.onValueChanged.RemoveAllListeners();
            operatorDropdown.onValueChanged.AddListener((value) => Sync());

            multiplyByDropdown.onValueChanged.RemoveAllListeners();
            multiplyByDropdown.onValueChanged.AddListener((value) => Sync());

            canvasGroup.interactable = false;
        }


        public void ActivateOrDeactivate(bool toggleValue)
        {
            canvasGroup.interactable = toggleValue;
            if(toggleValue)
                formulaUI.Formula.AddOperation(this);
            else
                formulaUI.Formula.RemoveOperation(this);
            Sync();
        }

        private void OnDestroy()
        {
            formulaUI.Formula.RemoveOperation(this);
        }


        private void Sync()
        {
            text.text = valuesSlider.value.ToString();
            formulaUI.Formula.Modify(this, channel =>
            {
                channel.group = groupUI.Group;
                channel.orderInGroup = transform.GetSiblingIndex();
                channel.value = valuesSlider.value;
                channel.multiplyWith = multiplyByDropdown.value switch
                {
                    1 => MultiplyWith.StartValue,
                    2 => MultiplyWith.StartGroupValue,
                    3 => MultiplyWith.CurrentValue,
                    _ => MultiplyWith.Nothing,
                };
                channel.op = operatorDropdown.value switch
                {
                    0 => Operator.Add,
                    1 => Operator.Subtract,
                    2 => Operator.Multiply,
                    3 => Operator.Divide,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return channel;
            });
        }
    }
}