using System;

namespace OverBang.GameName.Core.Menus
{
    public class SettingsUI : NavigablePanel
    {
        public event Action OnSettingsClosed;

        protected override void Awake()
        {
            base.Awake();
            OnBackClicked += () =>
            {
                OnSettingsClosed?.Invoke();
                Hide();
            };
        }
    }
}