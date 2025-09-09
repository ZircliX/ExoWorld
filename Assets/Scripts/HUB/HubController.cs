using System.Collections.Generic;
using OverBang.GameName.Managers;
using OverBang.GameName.Network;
using OverBang.GameName.Player;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.HUB
{
    public class HubController : NetworkBehaviour
    {
        [Header("Player UI Card")]
        [SerializeField] private PlayerCard playerCardPrefab;
        [SerializeField] private Transform playerCardContainer;
        
        [Header("Game Start")]
        [SerializeField] private Transform shipTransform;

        public NetworkList<PlayerHubState> PlayerStates { get; private set; } 
            = new NetworkList<PlayerHubState>(writePerm: NetworkVariableWritePermission.Server);
        
        private Dictionary<ulong, PlayerCard> playerCards;
        
        private void Awake()
        {
            playerCards = new Dictionary<ulong, PlayerCard>(4);
        }
        
        // --- Network Lifecycle ---
        public override void OnNetworkSpawn()
        {
            // Clients s'abonnent aux events
            PlayerStates.OnListChanged += OnPlayersListChanged;

            if (IsServer)
            {
                // Init de la liste avec les joueurs existants
                PlayerStates.Clear();
                foreach (ulong playerID in PlayerManager.Instance.Players.Keys)
                {
                    bool isReady = IsPlayerReady(playerID);
                    PlayerStates.Add(new PlayerHubState(playerID, isReady));
                }

                // Subscribes aux events PlayerManager côté serveur
                PlayerManager.Instance.OnPlayerRegistered += OnPlayerRegistered_Server;
                PlayerManager.Instance.OnPlayerUnregistered += OnPlayerUnregistered_Server;
                PlayerManager.Instance.OnPlayerReadyStatusChanged += OnPlayerReadyStatusChanged_Server;
            }
            
            if (IsClient)
            {
                foreach (PlayerHubState info in PlayerStates) // <- en late join, cette boucle sera remplie
                {
                    AddPlayerCard(info);
                }
            }
        }

        public override void OnNetworkDespawn()
        {
            PlayerStates.OnListChanged -= OnPlayersListChanged;

            if (IsServer && PlayerManager.HasInstance)
            {
                PlayerManager.Instance.OnPlayerRegistered -= OnPlayerRegistered_Server;
                PlayerManager.Instance.OnPlayerUnregistered -= OnPlayerUnregistered_Server;
                PlayerManager.Instance.OnPlayerReadyStatusChanged -= OnPlayerReadyStatusChanged_Server;
            }
        }

        private void OnPlayerRegistered_Server(ulong playerID)
        {
            if (!IsServer) return;
            PlayerStates.Add(new PlayerHubState(playerID, false));
        }

        private void OnPlayerUnregistered_Server(ulong playerID)
        {
            if (!IsServer) return;
            
            for (int i = 0; i < PlayerStates.Count; i++)
            {
                if (PlayerStates[i].PlayerID == playerID)
                {
                    PlayerStates.RemoveAt(i);
                    break;
                }
            }
        }

        private void OnPlayerReadyStatusChanged_Server(ulong playerID)
        {
            if (!IsServer) return;
            
            for (int i = 0; i < PlayerStates.Count; i++)
            {
                if (PlayerStates[i].PlayerID == playerID)
                {
                    PlayerStates[i] = new PlayerHubState(playerID, IsPlayerReady(playerID));
                    UpdatePlayerCard(PlayerStates[i]);
                    break;
                }
            }

            CheckForGameStartInternal();
        }

        // --- Client-side UI updates ---
        private void OnPlayersListChanged(NetworkListEvent<PlayerHubState> changeEvent)
        {
            switch (changeEvent.Type)
            {
                case NetworkListEvent<PlayerHubState>.EventType.Add:
                    AddPlayerCard(changeEvent.Value);
                    break;
                case NetworkListEvent<PlayerHubState>.EventType.Remove:
                    RemovePlayerCard(changeEvent.Value.PlayerID);
                    break;
                case NetworkListEvent<PlayerHubState>.EventType.Value:
                    Debug.Log("Update");
                    UpdatePlayerCard(changeEvent.Value);
                    break;
                case NetworkListEvent<PlayerHubState>.EventType.Clear:
                    foreach (PlayerCard card in playerCards.Values)
                        Destroy(card.gameObject);
                    playerCards.Clear();
                    break;
            }
        }

        private void AddPlayerCard(PlayerHubState info)
        {
            if (playerCards.ContainsKey(info.PlayerID)) return;

            PlayerCard card = Instantiate(playerCardPrefab, playerCardContainer);
            playerCards.Add(info.PlayerID, card);
            card.SetPlayerName($"Player {info.PlayerID}");
            card.SetPlayerStatus(info.IsReady ? "Ready" : "Not Ready");
        }

        private void RemovePlayerCard(ulong playerID)
        {
            if (!playerCards.TryGetValue(playerID, out PlayerCard card)) return;
            Destroy(card.gameObject);
            playerCards.Remove(playerID);
        }

        private void UpdatePlayerCard(PlayerHubState info)
        {
            if (!playerCards.TryGetValue(info.PlayerID, out PlayerCard card)) return;
            card.SetPlayerStatus(info.IsReady ? "Ready" : "Not Ready");
        }

        // --- Game start (server only) ---
        private void CheckForGameStartInternal()
        {
            if (!IsServer) return;
            if (PlayerStates.Count == 0) return;

            foreach (PlayerHubState player in PlayerStates)
            {
                Debug.LogError($"Player {player.PlayerID} is {player.IsReady}");
                if (!player.IsReady)
                    return;
            }

            Debug.LogWarning("[Hub] All PlayerStates ready -> starting game!");
            StartGameClientRpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void StartGameClientRpc()
        {
            Debug.Log("[Hub] Game starting");
            PlayerManager.Instance.TeleportPlayersRpc(shipTransform.position);
        }

        // --- Helpers ---
        private bool IsPlayerReady(ulong playerID)
        {
            if (PlayerManager.Instance.Players.TryGetValue(playerID, out PlayerController pc))
            {
                return pc.PlayerNetwork.IsReady.Value;
            }
            return false;
        }
    }
}