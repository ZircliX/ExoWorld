using System.Collections.Generic;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Menus
{
    public class Selector<T> : BaseSelector<T>
    {
        [SerializeField] private List<SelectorOption<T>> options = new List<SelectorOption<T>>();

        private int currentIndex;

        public override T CurrentValue => options.Count > 0 ? options[currentIndex].value : default;

        protected override void Awake()
        {
            base.Awake();
            UpdateDisplay();
        }

        protected override void OnNextClicked()
        {
            if (options.Count == 0) 
                return;
            
            currentIndex = (currentIndex + 1) % options.Count;
            UpdateDisplay();
            InvokeValueChanged();
        }

        protected override void OnPreviousClicked()
        {
            if (options.Count == 0) 
                return;
                
            currentIndex = (currentIndex - 1 + options.Count) % options.Count;
            UpdateDisplay();
            InvokeValueChanged();
        }

        protected override void UpdateDisplay()
        {
            if (options.Count > 0)
                displayText.text = options[currentIndex].displayName;
        }

        public void SetOptions(List<SelectorOption<T>> newOptions)
        {
            options = newOptions;
            currentIndex = 0;
            UpdateDisplay();
        }
    }
}