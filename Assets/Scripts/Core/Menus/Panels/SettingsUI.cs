using System;

namespace OverBang.ExoWorld.Core
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