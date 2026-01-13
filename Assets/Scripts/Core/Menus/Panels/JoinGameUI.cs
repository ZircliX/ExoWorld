using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.GameName.Core.Menus
{
    public class JoinGameUI : InteractivePanel
    {
        [SerializeField] private Button joinButton;
        [SerializeField] private RectTransform contentList;
        [SerializeField] private LobbyListItem lobbyItemPrefab;
        
        [SerializeField, Space] private ServerVisibilityToggle visibilityToggle;
        [SerializeField] private CanvasGroup publicCanvas;
        [SerializeField] private CanvasGroup privateCanvas;

        private LobbyInfo  selectedLobby;
        private List<LobbyListItem> lobbyItems;

        public event Action OnJoinGame;

        protected override void Awake()
        {
            base.Awake();
            OnBackClicked += Hide;
            visibilityToggle.OnFilterChanged += OnFilterChanged;
            
            joinButton.onClick.AddListener(HandleJoinGame);
            joinButton.interactable = false;
            
            lobbyItems = new List<LobbyListItem>();
        }

        private void OnDestroy()
        {
            visibilityToggle.OnFilterChanged -= OnFilterChanged;
        }

        private void OnFilterChanged(ServerVisibility visibility)
        {
            publicCanvas.gameObject.SetActive(visibility == ServerVisibility.Public);
            privateCanvas.gameObject.SetActive(visibility != ServerVisibility.Public);

            return;
            OpenCanvas(publicCanvas, visibility == ServerVisibility.Public);
            OpenCanvas(privateCanvas, visibility != ServerVisibility.Public);
        }

        public void DisplayLobbies(LobbyInfo[] lobbies)
        {
            ClearLobbies();

            foreach (LobbyInfo lobbyInfo in lobbies)
            {
                LobbyListItem item = CreateLobbyItem(lobbyInfo);
                lobbyItems.Add(item);
            }

            return;
            if (lobbyItems.Count > 0)
                firstSelectable = lobbyItems[0].GetComponent<Button>();
        }

        private LobbyListItem CreateLobbyItem(LobbyInfo lobby)
        {
            LobbyListItem itemGameObject = Instantiate(lobbyItemPrefab, contentList);
            itemGameObject.Setup(lobby, OnLobbySelected);

            return itemGameObject;
        }

        private void OnLobbySelected(LobbyInfo lobby)
        {
            selectedLobby = lobby;
            joinButton.interactable = true;
        }

        private void HandleJoinGame()
        {
            if (!string.IsNullOrEmpty(selectedLobby.lobbyId))
                OnJoinGame?.Invoke();
        }

        private void ClearLobbies()
        {
            foreach (LobbyListItem item in lobbyItems)
                Destroy(item.gameObject);

            lobbyItems.Clear();
        }

        protected override void OnHide()
        {
            ClearLobbies();
        }
    }
}