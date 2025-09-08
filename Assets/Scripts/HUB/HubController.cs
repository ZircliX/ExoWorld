using System.Collections.Generic;
using OverBang.GameName.Managers;
using OverBang.GameName.Network.Static;
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

        private Dictionary<byte, PlayerCard> playerCards;
        
        private void OnEnable()
        {
            if (PlayerManager.HasInstance)
            {
                SubscribeToPlayerManagerEvents();
            }
            else
            {
                PlayerManager.OnInstanceCreated += SubscribeToPlayerManagerEvents;
            }
        }
        
        private void OnDisable()
        {
            if (!PlayerManager.HasInstance) return;
            
            PlayerManager.Instance.OnPlayerRegistered -= OnPlayerRegistered;
            PlayerManager.Instance.OnPlayerUnregistered -= OnPlayerUnregistered;
            PlayerManager.Instance.OnPlayerReadyStatusChanged -= OnPlayerReadyStatusChanged;
        }
        
        private void SubscribeToPlayerManagerEvents()
        {
            PlayerManager.Instance.OnPlayerRegistered += OnPlayerRegistered;
            PlayerManager.Instance.OnPlayerUnregistered += OnPlayerUnregistered;
            PlayerManager.Instance.OnPlayerReadyStatusChanged += OnPlayerReadyStatusChanged;
            
            PlayerManager.OnInstanceCreated -= SubscribeToPlayerManagerEvents;
        }

        private void Awake()
        {
            playerCards = new Dictionary<byte, PlayerCard>(4);
        }

        public override void OnNetworkSpawn()
        {
            if (PlayerManager.HasInstance && PlayerManager.Instance.IsSpawned)
            {
                InitializeHubRpc();
            }
            else
            {
                // Manager not yet ready, subscribe and wait
                PlayerManager.OnInstanceCreated += InitializeHubRpc;
            }
        }

        [Rpc(SendTo.Server)]
        private void InitializeHubRpc()
        {
            foreach (byte player in PlayerManager.Instance.PlayerIDs)
            {
                byte playerID = player;
                bool isReady = IsPlayerReady(playerID);
                
                OnPlayerRegisteredRpc(playerID, isReady);
            }
            
            PlayerManager.OnInstanceCreated -= InitializeHubRpc;
        }

        private void OnPlayerRegistered(byte playerID)
        {
            if (!IsOwner) return;
            OnPlayerRegisteredRpc(playerID, false);
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void OnPlayerRegisteredRpc(byte playerID, bool isReady)
        {
            StartCoroutine(this.CanRunNetworkOperation(() =>
            {
                if (playerCards.ContainsKey(playerID)) return;
                
                PlayerCard card = Instantiate(playerCardPrefab, playerCardContainer);
                playerCards.Add(playerID, card);

                card.SetPlayerName($"Player {playerID}");
                
                SetPlayerReadyStatusChangedRpc(playerID, isReady);
            }));
        }

        private void OnPlayerUnregistered(byte playerID)
        {
            StartCoroutine(this.CanRunNetworkOperation(() =>
            {
                OnPlayerUnregisteredRpc(playerID);
            }));
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void OnPlayerUnregisteredRpc(byte playerID)
        {
            if (!playerCards.TryGetValue(playerID, out PlayerCard card))
            {
                Debug.LogWarning($"PlayerCard for {playerID} not found in dictionary.");
                return;
            }

            Destroy(card.gameObject);
            playerCards.Remove(playerID);
        }
        
        // --- Player Ready ---

        private void OnPlayerReadyStatusChanged(byte playerID)
        {
            StartCoroutine(this.CanRunNetworkOperation(() =>
            {
                bool isReady = IsPlayerReady(playerID);
                SetPlayerReadyStatusChangedRpc(playerID, isReady);
                
                if (IsServer)
                {
                    CheckForGameStartInternal();
                }
                else
                {
                    CheckForGameStartRpc();
                }
            }));
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void SetPlayerReadyStatusChangedRpc(byte playerID, bool isReady)
        {
            if (!playerCards.TryGetValue(playerID, out PlayerCard card))
            {
                Debug.LogWarning($"PlayerCard for {playerID} not found in dictionary.");
                return;
            }

            card.SetPlayerStatus(isReady ? "Ready" : "Not Ready");
        }

        [Rpc(SendTo.Server)]
        private void CheckForGameStartRpc()
        {
            CheckForGameStartInternal();
        }

        //OnlyServer
        private void CheckForGameStartInternal()
        {
            if (playerCards.Count == 0) return;

            foreach (KeyValuePair<byte, PlayerController> playerInfo in PlayerManager.Instance.Players)
            {
                Debug.LogError($"Player {playerInfo.Key} has ready status: {playerInfo.Value.PlayerNetwork.IsReady.Value}");
                if (!playerInfo.Value.PlayerNetwork.IsReady.Value) return;
            }
            
            StartShipRpc();
        }

        private void StartShipRpc()
        {
            Debug.LogWarning("All players are ready, starting game !");
            PlayerManager.Instance.TeleportPlayersRpc(shipTransform.position);
        }
        
        // --- Helpers Methods ---

        private bool IsPlayerReady(byte playerID)
        {
            bool ready = false;
            if (PlayerManager.Instance.Players.TryGetValue(playerID, out PlayerController pc))
            {
                ready = pc.PlayerNetwork.IsReady.Value;
            }
            
            return ready;
        }
    }
}