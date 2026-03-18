using System;
using System.Collections.Generic;
using Helteix.Tools;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Core.Utils;
using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core.Menus
{
    public class JoinGameUI : NavigablePanel
    {
        // Public UI
        [SerializeField] private Button joinButton;
        [SerializeField] private RectTransform contentList;
        [SerializeField] private LobbyListItem lobbyItemPrefab;
        
        // Private UI
        [SerializeField, Space] private TMP_InputField passwordInput;
        
        [SerializeField, Space] private ServerVisibilityToggle visibilityToggle;
        [SerializeField] private CanvasGroup publicCanvas;
        [SerializeField] private CanvasGroup privateCanvas;
        
        private SessionInfo selectedSession;
        private List<LobbyListItem> lobbyItems;

        public Action OnJoinedGame;
        public event JoinGameBySession OnJoinGameBySessionRequested;
        public event JoinGameByCode OnJoinGameByCodeRequested;
        public delegate void JoinGameBySession(SessionInfo info);
        public delegate void JoinGameByCode(string code);

        protected override void Awake()
        {
            base.Awake();
            OnBackClicked += Hide;
            visibilityToggle.OnFilterChanged += OnFilterChanged;
            
            joinButton.onClick.AddListener(HandleJoinGame);
            joinButton.interactable = false;
            
            passwordInput.onValueChanged.AddListener(HandlePasswordChanged);
            passwordInput.characterLimit = GameMetrics.Global.MaxPasswordLenght;
            
            lobbyItems = new List<LobbyListItem>();
            contentList.ClearChildren();
        }

        private void OnDestroy()
        {
            visibilityToggle.OnFilterChanged -= OnFilterChanged;
            joinButton.onClick.RemoveAllListeners();
            passwordInput.onValueChanged.RemoveAllListeners();
        }

        private void OnFilterChanged(ServerVisibility visibility)
        {
            publicCanvas.gameObject.SetActive(visibility == ServerVisibility.Public);
            privateCanvas.gameObject.SetActive(visibility != ServerVisibility.Public);
            
            joinButton.interactable = false;
            selectedSession = default;

            if (visibility == ServerVisibility.Public)
            {
                DisplayLobbies();
            }
        }

        private async void DisplayLobbies()
        {
            try
            {
                // Query for available sessions
                QuerySessionsOptions queryOptions = new QuerySessionsOptions()
                {
                    Count = 10,
                    FilterOptions = new List<FilterOption>()
                    {
                        new FilterOption(FilterField.AvailableSlots, "1", FilterOperation.GreaterOrEqual),
                        new FilterOption(FilterField.IsLocked, "false", FilterOperation.Equal)
                    }
                };

                IList<ISessionInfo> availableSessions = await SessionManager.Global.QuerySessions(queryOptions);

                // Convert ISession to LobbyInfo
                List<SessionInfo> filteredLobbies = new List<SessionInfo>(availableSessions.Count);
                foreach (ISessionInfo session in availableSessions)
                {
                    SessionInfo info = new SessionInfo()
                    {
                        sessionId = session.Id,
                        sessionName = session.Name,
                        currentPlayers = session.MaxPlayers - session.AvailableSlots,
                        maxPlayers = session.MaxPlayers,
                    };
                    filteredLobbies.Add(info);
                }
            
                // Display sessions
                ClearLobbies();

                foreach (SessionInfo lobbyInfo in filteredLobbies)
                {
                    LobbyListItem item = CreateLobbyItem(lobbyInfo);
                    lobbyItems.Add(item);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to query sessions: {e}");
            }
        }

        private LobbyListItem CreateLobbyItem(SessionInfo session)
        {
            LobbyListItem itemGameObject = Instantiate(lobbyItemPrefab, contentList);
            itemGameObject.Initialize(session, OnSessionSelected);

            return itemGameObject;
        }

        private void OnSessionSelected(SessionInfo session)
        {
            selectedSession = session;
            joinButton.interactable = true;
        }

        private void HandleJoinGame()
        {
            if (!string.IsNullOrEmpty(passwordInput.text))
            {
                //Debug.Log($"Joining game with password: {passwordInput.text}");
                OnJoinGameByCodeRequested?.Invoke(passwordInput.text);
            }

            if (!string.IsNullOrEmpty(selectedSession.sessionName))
            {
                //Debug.Log($"Joining game with session ID: {selectedSession.sessionId}");   
                OnJoinGameBySessionRequested?.Invoke(selectedSession);
            }
            
            LoadingUI.Instance.Open();
        }

        private void HandlePasswordChanged(string current)
        {
            joinButton.interactable = !string.IsNullOrEmpty(current) && current.Length == GameMetrics.Global.MaxPasswordLenght;
        }

        private void ClearLobbies()
        {
            foreach (LobbyListItem item in lobbyItems)
                Destroy(item.gameObject);

            lobbyItems.Clear();
            joinButton.interactable = false;
        }

        protected override void OnShow()
        {
            DisplayLobbies();
        }

        protected override void OnHide()
        {
            ClearLobbies();
        }
    }
}