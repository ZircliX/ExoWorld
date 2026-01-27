using System;
using OverBang.ExoWorld.Core.Metrics;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core.Menus
{
    public class MainMenuPanel : BasePanel
    {
        [SerializeField] private Button hostButton;
        [SerializeField] private Button joinButton;
        [SerializeField] private Button contactButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        
        [SerializeField] private Button discordButton;
        [SerializeField] private Button instagramButton;

        public event Action OnHostClicked;
        public event Action OnJoinClicked;
        public event Action OnContactClicked;
        public event Action OnSettingsClicked;

        protected override void Awake()
        {
            base.Awake();
            SubscribeToButtons();
            ConfigureNavigation();
        }

        private void SubscribeToButtons()
        {
            hostButton.onClick.AddListener(() => OnHostClicked?.Invoke());
            joinButton.onClick.AddListener(() => OnJoinClicked?.Invoke());
            contactButton.onClick.AddListener(() => OnContactClicked?.Invoke());
            settingsButton.onClick.AddListener(() => OnSettingsClicked?.Invoke());
            quitButton.onClick.AddListener(Application.Quit);
            
            discordButton.onClick.AddListener(() => Application.OpenURL(GameMetrics.Global.ConstID.DiscordLink));
            instagramButton.onClick.AddListener(() => Application.OpenURL(GameMetrics.Global.ConstID.InstagramLink));
        }

        private void ConfigureNavigation()
        {
            hostButton.navigation = CreateNavigation(down: joinButton);
            joinButton.navigation = CreateNavigation(up: hostButton, down: settingsButton);
            settingsButton.navigation = CreateNavigation(up: joinButton);
        }
    }
}