using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.GameName.Core.Menus
{
    public class WaitingScreenUI : NavigablePanel
    {
        [SerializeField] private Button startButton;
        [SerializeField] private PlayerListItem playerListItemPrefab;
        [SerializeField] private Transform playersContainer;
        
        private List<PlayerListItem> playerListItems;

        protected override void OnShow()
        {
            startButton.onClick.AddListener(OnCreateGame);

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }

        private void OnClientConnected(ulong obj)
        {
            foreach (PlayerListItem playerListItem in playerListItems)
            {
                Destroy(playerListItem.gameObject);
            }
            
            foreach (IReadOnlyPlayer player in SessionManager.Global.ActiveSession.Players)
            {
                PlayerListItem item = Instantiate(playerListItemPrefab, playersContainer);
                item.Initialize(player.Id);
            }
        }

        public override async void InvokeBackClicked()
        {
            await SessionManager.Global.LeaveCurrentSession();
            OnBackClicked?.Invoke();
        }

        private void OnCreateGame()
        {
            if (!SessionManager.Global.IsHost()) 
                return;
            
            IHostSession session = (IHostSession)SessionManager.Global.ActiveSession;
            session.IsLocked = true;
            
            CreateGameModeRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void CreateGameModeRpc()
        {
            if (GameModeUtils.TryGetGameModeForName(GameModeUtils.SurvivalGameModeName, out Type gameModeType))
            {
                object gameMode = Activator.CreateInstance(gameModeType);
                if (gameMode is IGameMode gameModeInstance)
                    gameModeInstance.SetGameMode();
            }
        }
    }
}