using System;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.GameName.Core.Menus
{
    public class MainMenuPanel : BasePanel
    {
        [SerializeField] private Button hostButton;
        [SerializeField] private Button joinButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        
        [SerializeField] private Button discordButton;
        [SerializeField] private Button instagramButton;

        public event Action OnHostClicked;
        public event Action OnJoinClicked;
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
            settingsButton.onClick.AddListener(() => OnSettingsClicked?.Invoke());
            quitButton.onClick.AddListener(Application.Quit);
            
            discordButton.onClick.AddListener(() => Application.OpenURL("https://discord.gg/WsxzTKC2uU"));
            instagramButton.onClick.AddListener(() => Application.OpenURL("https://www.instagram.com/overbangstudio"));
        }

        private void ConfigureNavigation()
        {
            hostButton.navigation = CreateNavigation(down: joinButton);
            joinButton.navigation = CreateNavigation(up: hostButton, down: settingsButton);
            settingsButton.navigation = CreateNavigation(up: joinButton);
        }
    }
}