using System;
using System.Collections.Generic;
using Helteix.Tools;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Core.Utils;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core.Menus
{
    public class WaitingScreenUI : NavigablePanel
    {
        [SerializeField, Required] private Button startButton;
        [SerializeField, Required] private Button codeButton;
        [SerializeField, Required] private PlayerListItem playerListItemPrefab;
        [SerializeField, Required] private Transform playersContainer;
        [SerializeField, Required] private LayoutGroup playersContainerLayoutGroup;
        
        private List<PlayerListItem> playerListItems;
        private TMP_Text codeText;

        public event Action OnStartGameRpc;

        protected override void Awake()
        {
            base.Awake();
            playerListItems = new List<PlayerListItem>(4);
            playersContainer.ClearChildren();
            codeText = codeButton.GetComponentInChildren<TMP_Text>();
        }

        protected override void OnShow()
        {
            startButton.onClick.AddListener(OnCreateGame);
            startButton.interactable = SessionManager.Global.IsHost();
            
            IHostSession session = (IHostSession)SessionManager.Global.ActiveSession;
            codeText.text = session.Code;
            codeButton.onClick.AddListener(() => GUIUtility.systemCopyBuffer = codeText.text);

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            OnClientConnected();
        }

        protected override void OnHide()
        {
            ClearPlayerList();
        }

        private void OnClientConnected(ulong obj = 0)
        {
            ClearPlayerList();

            IReadOnlyList<IReadOnlyPlayer> players = SessionManager.Global.ActiveSession.Players;
            
            if (players.Count == 0)
            {
                Debug.LogWarning("No players in session");
                return;
            }

            foreach (IReadOnlyPlayer player in players)
            {
                if (player == null)
                    continue;

                PlayerListItem item = Instantiate(playerListItemPrefab, playersContainer);
                
                string playerName = GetPlayerName(player);
                item.Initialize(playerName);
                
                playerListItems.Add(item);
            }
            
            if (playersContainerLayoutGroup != null)
            {
                playersContainerLayoutGroup.SetLayoutHorizontal();
                playersContainerLayoutGroup.SetLayoutVertical();
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)playersContainer);
        }
        
        private void ClearPlayerList()
        {
            foreach (PlayerListItem playerListItem in playerListItems)
            {
                Destroy(playerListItem.gameObject);
            }
            playerListItems.Clear();
        }
        
        private string GetPlayerName(IReadOnlyPlayer player)
        {
            bool hasName = player.TryGetPlayerProperty(
                GameMetrics.Global.ConstID.PlayerPropertyPlayerName, 
                out string playerName);

            return hasName && !string.IsNullOrEmpty(playerName) 
                ? playerName 
                : $"Player_{player.Id[..6]}";
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
            OnStartGameRpc?.Invoke();
        }
    }
}