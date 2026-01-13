using System;

namespace OverBang.GameName.Core.Menus
{
    public class InGameUI : NavigablePanel
    {
        public event Action OnGameLeft;

        protected override void Awake()
        {
            base.Awake();
            OnBackClicked += () => OnGameLeft?.Invoke();
        }
    }
}