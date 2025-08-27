using System.Collections;
using System.Collections.Generic;
using OverBang.GameName.Managers;
using OverBang.GameName.Network.Static;
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
            PlayerManager.OnInstanceCreated += SubscribeToPlayerManagerEvents;
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
            if (!this.CanRunNetworkOperation())
            {
                Debug.LogWarning($"[HubController] Cannot Register player {playerID}.");
                return;
            }
            
            if (IsServer)
            {
                OnPlayerRegisteredInternal(playerID);
            }
            else
            {
                OnPlayerRegisteredRpc(playerID);
            }
        }

        private void OnPlayerRegisteredInternal(byte playerID)
        {
            
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void OnPlayerRegisteredRpc(byte playerID)
        {
            if (playerCards.ContainsKey(playerID)) return;
            
            PlayerCard card = Instantiate(playerCardPrefab, playerCardContainer);
            playerCards.Add(playerID, card);

            card.SetPlayerName($"Player {playerCards.Count}");
            SetPlayerReadyStatusRpc(playerID, false);
        }

        private void OnPlayerUnregistered(byte playerID)
        {
            if (this.CanRunNetworkOperation())
            {
                OnPlayerUnregisteredRpc(playerID);
            }
            else
            {
                Debug.LogWarning($"[HubController] Cannot Unregister player {playerID}.");
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void OnPlayerUnregisteredRpc(byte playerID)
        {
            if (playerCards.TryGetValue(playerID, out PlayerCard card))
            {
                Destroy(card.gameObject);
                playerCards.Remove(playerID);
            }
            else
            {
                Debug.LogWarning($"PlayerCard for {playerID} not found in dictionary.");
            }
        }

        private void OnPlayerReadyStatusChanged(byte playerID, bool readyStatus)
        {
            if (this.CanRunNetworkOperation())
            {
                SetPlayerReadyStatusRpc(playerID, readyStatus);
                CheckForGameStart();
            }
            else
            {
                Debug.LogWarning($"[HubController] Cannot change player's ready status {playerID}.");
            }
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void SetPlayerReadyStatusRpc(byte playerID, bool readyStatus)
        {
            if (playerCards.TryGetValue(playerID, out PlayerCard card))
            {
                card.SetPlayerStatus(readyStatus ? "Ready" : "Not Ready");
            }
            else
            {
                Debug.LogWarning($"PlayerCard for {playerID} not found in dictionary.");
            }
        }

        private void CheckForGameStart()
        {
            if (playerCards.Count == 0) return;

            foreach (KeyValuePair<byte, PlayerCard> playerInfo in playerCards)
            {
                if (playerInfo.Value.PlayerStatus.text != "Ready") return;
            }
            
            StartCoroutine(StartShip());
        }

        private IEnumerator StartShip()
        {
            yield return null;
        }
    }
}