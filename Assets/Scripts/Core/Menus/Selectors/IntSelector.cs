using UnityEngine;

namespace OverBang.GameName.Core.Menus
{
    public class IntSelector : BaseSelector<int>
    {
        [SerializeField] private int minValue = 1;
        [SerializeField] private int maxValue = 8;

        private int currentValue;

        public override int CurrentValue => currentValue;

        protected override void Awake()
        {
            base.Awake();
            currentValue = minValue;
            UpdateDisplay();
        }

        protected override void OnNextClicked()
        {
            if (currentValue >= maxValue)
                return;
            
            currentValue++;
            UpdateDisplay();
            InvokeValueChanged();
        }

        protected override void OnPreviousClicked()
        {
            if (currentValue <= minValue)
                return;
            
            currentValue--;
            UpdateDisplay();
            InvokeValueChanged();
        }

        protected override void UpdateDisplay()
        {
            displayText.text = currentValue.ToString();
        }

        public void SetRange(int min, int max)
        {
            minValue = min;
            maxValue = max;
            currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
            UpdateDisplay();
        }
    }
}