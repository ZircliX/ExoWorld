using System;
using System.Collections.Generic;
using OverBang.GameName.Gameplay.Player;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Hub
{
    public class HubController : NetworkBehaviour
    {
        [Header("Player UI Card")]
        [SerializeField] private PlayerCard playerCardPrefab;
        [SerializeField] private Transform playerCardContainer;

        [Header("Game Start")]
        [SerializeField] private Transform shipTransform;

        private Dictionary<ulong, PlayerCard> playerCards;
        
        public event Action<ulong> OnHubPlayerAdded;
        public event Action<ulong> OnHubPlayerRemoved;
        public event Action<ulong, bool> OnHubPlayerReadyChanged;
        
        private void OnEnable()
        {
            if (PlayerManager.HasInstance)
            {
                SubscribeToEvents();
            }
            else
            {
                PlayerManager.OnInstanceCreated += SubscribeToEvents;
            }
        }

        private void SubscribeToEvents()
        {
            PlayerManager.OnInstanceCreated -= SubscribeToEvents;
            
            PlayerManager manager = PlayerManager.Instance;
            manager.OnPlayerRegistered += HandlePlayerRegistered;
            manager.OnPlayerUnregistered += HandlePlayerUnregistered;
            manager.OnPlayerReadyStatusChanged += HandlePlayerReadyChanged;
        }

        private void OnDisable()
        {
            if (!PlayerManager.HasInstance) return;
            
            PlayerManager manager = PlayerManager.Instance;
            manager.OnPlayerRegistered -= HandlePlayerRegistered;
            manager.OnPlayerUnregistered -= HandlePlayerUnregistered;
            manager.OnPlayerReadyStatusChanged -= HandlePlayerReadyChanged;
        }

        private void Awake()
        {
            playerCards = new Dictionary<ulong, PlayerCard>(4);
        }
        
        private void HandlePlayerRegistered(ulong playerId)
        {
            AddPlayerCard(playerId, false);
            OnHubPlayerAdded?.Invoke(playerId);
        }

        private void HandlePlayerUnregistered(ulong playerId)
        {
            RemovePlayerCard(playerId);
            OnHubPlayerRemoved?.Invoke(playerId);
        }

        private void HandlePlayerReadyChanged(ulong playerId, bool isReady)
        {
            UpdatePlayerCard(playerId, isReady);
            OnHubPlayerReadyChanged?.Invoke(playerId, isReady);
        }
        
        // --- Gameplay UI updates ---
        public void AddPlayerCard(ulong playerId, bool isReady)
        {
            if (playerCards.ContainsKey(playerId)) return;

            PlayerCard card = Instantiate(playerCardPrefab, playerCardContainer);
            playerCards.Add(playerId, card);
            card.SetPlayerName($"Player {playerId}");
            card.SetPlayerStatus(isReady ? "Ready" : "Not Ready");
        }

        public void RemovePlayerCard(ulong playerId)
        {
            if (!playerCards.TryGetValue(playerId, out PlayerCard card)) return;
            Destroy(card.gameObject);
            playerCards.Remove(playerId);
        }

        public void UpdatePlayerCard(ulong playerId, bool isReady)
        {
            if (!playerCards.TryGetValue(playerId, out PlayerCard card)) return;
            card.SetPlayerStatus(isReady ? "Ready" : "Not Ready");
        }

        public void ClearPlayerCards()
        {
            foreach (PlayerCard card in playerCards.Values)
                Destroy(card.gameObject);
            playerCards.Clear();
        }
        
        // --- Runtime Logic ---

        public void StartGame()
        {
            Debug.Log("[Hub] Game starting!");
            // Offline: directly teleport players
            foreach (KeyValuePair<ulong, PlayerController> kvp in PlayerManager.Instance.Players)
            {
                PlayerManager.Instance.TeleportPlayer(kvp.Key, shipTransform.position);
            }
        }
    }
}