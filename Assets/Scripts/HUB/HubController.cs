using System.Collections;
using System.Collections.Generic;
using OverBang.GameName.Managers;
using OverBang.GameName.Metrics;
using OverBang.GameName.Network.Static;
using OverBang.GameName.Player;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.HUB
{
    public class HubController : NetworkBehaviour
    {
        [SerializeField] private PlayerCard playerCardPrefab;
        [SerializeField] private Transform playerCardContainer;

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
                InitializeHub();
            }
            else
            {
                // Manager not yet ready, subscribe and wait
                PlayerManager.OnInstanceCreated += InitializeHub;
            }
        }

        private void InitializeHub()
        {
            if (IsClient)
            {
                foreach (byte player in PlayerManager.Instance.Players)
                {
                    OnPlayerRegisteredRpc(player);
                }
            }
            
            PlayerManager.OnInstanceCreated -= InitializeHub;
        }
        
        private void OnPlayerRegistered(byte playerID)
        {
            StartCoroutine(this.CanRunNetworkOperation(() =>
            {
                OnPlayerRegisteredRpc(playerID);
            }));
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void OnPlayerRegisteredRpc(byte playerID)
        {
            Debug.Log(playerCards);
            if (playerCards.ContainsKey(playerID)) return;
            
            PlayerCard card = Instantiate(playerCardPrefab, playerCardContainer);
            playerCards.Add(playerID, card);

            card.SetPlayerName($"Player {playerCards.Count}");
            SetPlayerReadyStatusChangedRpc(playerID, false);
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

        private void OnPlayerReadyStatusChanged(byte playerID, bool readyStatus)
        {
            StartCoroutine(this.CanRunNetworkOperation(() =>
            {
                Debug.Log($"Player {playerID} to {readyStatus}");
                SetPlayerReadyStatusChangedRpc(playerID, readyStatus);
            
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
        private void SetPlayerReadyStatusChangedRpc(byte playerID, bool readyStatus)
        {
            if (!playerCards.TryGetValue(playerID, out PlayerCard card))
            {
                Debug.LogWarning($"PlayerCard for {playerID} not found in dictionary.");
                return;
            }

            card.SetPlayerStatus(readyStatus ? "Ready" : "Not Ready");
        }

        [Rpc(SendTo.Server)]
        private void CheckForGameStartRpc()
        {
            CheckForGameStartInternal();
        }

        private void CheckForGameStartInternal()
        {
            if (playerCards.Count == 0) return;

            foreach (KeyValuePair<byte, PlayerController> playerInfo in PlayerManager.Instance.PlayerControllers)
            {
                Debug.LogError($"Player {playerInfo.Key} has ready status: {playerInfo.Value.PlayerNetwork.IsReady.Value}");
                if (!playerInfo.Value.PlayerNetwork.IsReady.Value) return;
            }
            
            StartShipRpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void StartShipRpc()
        {
            Debug.LogWarning("Waaaaaaaa");
        }
    }
}