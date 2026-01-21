using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core
{
    public class HostGameUI : NavigablePanel
    {
        [SerializeField, Space] private TMP_InputField gameNameInput;
        [SerializeField] private IntSelector maxPlayersSelector;
        [SerializeField] private ServerVisibilitySelector serverVisibilitySelector;
        [SerializeField] private TMP_Text passwordText;
        
        [SerializeField, Space] private Button createButton;
        [SerializeField] private Button copyButton;

        public Action OnHostCreated;
        public event HostCreatedEvent OnCreateHostClicked;
        public delegate void HostCreatedEvent(string serverName, int maxPlayers, ServerVisibility visibility, string password);

        private string currentPassword;

        protected override void Awake()
        {
            base.Awake();
            OnBackClicked += Hide;
            
            copyButton.onClick.AddListener(() => GUIUtility.systemCopyBuffer = currentPassword);
            
            createButton.onClick.AddListener(
                () => HandleHostCreate(gameNameInput.text, 
                    maxPlayersSelector.CurrentValue, 
                    serverVisibilitySelector.CurrentValue, 
                    currentPassword)
                );

            createButton.interactable = false;
            gameNameInput.onValueChanged.AddListener(text => createButton.interactable = !string.IsNullOrEmpty(text));
        }

        private void HandleHostCreate(string serverName, int maxPlayers, ServerVisibility visibility, string password)
        {
            serverName = serverName.Trim().Replace(" ", string.Empty);
            gameNameInput.text = serverName;
            
            OnCreateHostClicked?.Invoke(serverName, maxPlayers, visibility, password);
        }

        private static readonly StringBuilder builder = new StringBuilder();
        protected override void OnShow()
        {
            // Generate a random password
            builder.Clear();

            for (int i = 0; i < GameMetrics.Global.MaxPasswordLenght; i++)
            {
                char c = (char)UnityEngine.Random.Range(65, 91);
                builder.Append(char.ToUpper(c));
            }

            currentPassword = builder.ToString();
            passwordText.text = currentPassword;
        }
    }
}