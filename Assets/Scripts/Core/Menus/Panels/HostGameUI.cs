using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core.Menus
{
    public class HostGameUI : NavigablePanel
    {
        [SerializeField, Space] private TMP_InputField gameNameInput;
        [SerializeField] private IntSelector maxPlayersSelector;
        [SerializeField] private ServerVisibilitySelector serverVisibilitySelector;
        
        [SerializeField, Space] private Button createButton;

        public Action OnHostCreated;
        public event HostCreatedEvent OnCreateHostClicked;
        public delegate void HostCreatedEvent(string serverName, int maxPlayers, ServerVisibility visibility);

        private string currentPassword;
        private bool started;

        protected override void Awake()
        {
            base.Awake();
            OnBackClicked += Hide;
            
            createButton.onClick.AddListener(
                () =>
                {
                    HandleHostCreate(gameNameInput.text,
                        maxPlayersSelector.CurrentValue,
                        serverVisibilitySelector.CurrentValue);
                    createButton.interactable = false;
                    started = true;
                });

            createButton.interactable = false;
            gameNameInput.onValueChanged.AddListener(text => createButton.interactable = !string.IsNullOrEmpty(text) && !started);
        }

        private void HandleHostCreate(string serverName, int maxPlayers, ServerVisibility visibility)
        {
            serverName = serverName.Trim().Replace(" ", string.Empty);
            gameNameInput.text = serverName;
            
            OnCreateHostClicked?.Invoke(serverName, maxPlayers, visibility);
        }

        protected override void OnShow()
        {
            started = false;
        }
    }
}