using System;

namespace OverBang.GameName.Core.Menus
{
    public class EnumSelector<T> : BaseSelector<T> where T : System.Enum
    {
        private T[] enumValues;
        private int currentIndex;

        public override T CurrentValue => enumValues[currentIndex];

        protected override void Awake()
        {
            base.Awake();
            enumValues = Enum.GetValues(typeof(T)) as T[];
            currentIndex = 0;
            UpdateDisplay();
        }

        protected override void OnNextClicked()
        {
            currentIndex = (currentIndex + 1) % enumValues.Length;
            UpdateDisplay();
            InvokeValueChanged();
        }

        protected override void OnPreviousClicked()
        {
            currentIndex = (currentIndex - 1 + enumValues.Length) % enumValues.Length;
            UpdateDisplay();
            InvokeValueChanged();
        }

        protected override void UpdateDisplay()
        {
            displayText.text = enumValues[currentIndex].ToString();
        }
    }
}