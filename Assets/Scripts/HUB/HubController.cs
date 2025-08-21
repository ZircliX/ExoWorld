using System.Collections.Generic;
using OverBang.GameName.Managers;
using OverBang.GameName.Network.Static;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.HUB
{
    public class HubController : NetworkBehaviour
    {
        [SerializeField] private PlayerCard playerCardPrefab;
        [SerializeField] private Transform playerCardContainer;

        private Dictionary<string, PlayerCard> playerCards;

        private void OnEnable()
        {
            PlayerManager.OnInstanceCreated += SubscribeToPlayerManagerEvents;
        }
        
        private void OnDisable()
        {
            if (!PlayerManager.HasInstance) return;
            
            PlayerManager.Instance.OnPlayerRegistered -= OnPlayerRegistered;
            PlayerManager.Instance.OnPlayerUnregistered -= OnPlayerUnregistered;
        }
        
        private void SubscribeToPlayerManagerEvents()
        {
            PlayerManager.Instance.OnPlayerRegistered += OnPlayerRegistered;
            PlayerManager.Instance.OnPlayerUnregistered += OnPlayerUnregistered;
            
            PlayerManager.OnInstanceCreated -= SubscribeToPlayerManagerEvents;
        }

        private void Awake()
        {
            playerCards = new Dictionary<string, PlayerCard>(4);
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
                foreach (FixedString64Bytes player in PlayerManager.Instance.Players)
                {
                    OnPlayerRegisteredRpc(player.ToString());
                }
                
                PlayerManager.OnInstanceCreated -= InitializeHub;
            }
        }
        
        private void OnPlayerRegistered(string guid)
        {
            if (this.CanRunNetworkOperation())
            {
                OnPlayerRegisteredRpc(guid);
            }
            else
            {
                Debug.LogWarning($"[HubController] Cannot Register player {guid}. NetworkManager inactive.");
            }
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void OnPlayerRegisteredRpc(string guid)
        {
            if (playerCards.ContainsKey(guid)) return;
            
            PlayerCard card = Instantiate(playerCardPrefab, playerCardContainer);
            playerCards.Add(guid, card);

            card.SetPlayerName($"Player {playerCards.Count}");
            SetPlayerReadyStatus(guid, false);
        }

        private void OnPlayerUnregistered(string guid)
        {
            if (this.CanRunNetworkOperation())
            {
                OnPlayerUnregisteredRpc(guid);
            }
            else
            {
                Debug.LogWarning($"[HubController] Cannot Unregister player {guid}. NetworkManager inactive.");
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void OnPlayerUnregisteredRpc(string guid)
        {
            if (playerCards.TryGetValue(guid, out PlayerCard card))
            {
                Destroy(card.gameObject);
                playerCards.Remove(guid);
            }
            else
            {
                Debug.LogWarning($"PlayerCard for {guid} not found in dictionary.");
            }
        }

        private void SetPlayerReadyStatus(string guid, bool isReady)
        {
            if (playerCards.TryGetValue(guid, out PlayerCard card))
            {
                card.SetPlayerStatus(isReady ? "Ready" : "Not Ready");
            }
            else
            {
                Debug.LogWarning($"PlayerCard for {guid} not found in dictionary.");
            }
        }
    }
}