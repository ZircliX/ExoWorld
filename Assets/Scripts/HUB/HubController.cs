using System.Collections.Generic;
using OverBang.GameName.Managers;
using OverBang.GameName.Player;
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
            PlayerManager.Instance.OnPlayerRegistered += OnPlayerRegisteredRpc;
            PlayerManager.Instance.OnPlayerUnregistered += OnPlayerUnregisteredRpc;
        }
        
        private void OnDisable()
        {
            PlayerManager.Instance.OnPlayerRegistered -= OnPlayerRegisteredRpc;
            PlayerManager.Instance.OnPlayerUnregistered -= OnPlayerUnregisteredRpc;
        }
        
        public override void OnNetworkSpawn()
        {
            playerCards = new Dictionary<string, PlayerCard>(4);
            
            if (IsClient)
            {
                foreach (PlayerController player in PlayerManager.Instance.Players)
                {
                    OnPlayerRegisteredRpc(player.Guid);
                }
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

        [Rpc(SendTo.ClientsAndHost)]
        private void OnPlayerRegisteredRpc(string guid)
        {
            PlayerCard card = Instantiate(playerCardPrefab, playerCardContainer);
            playerCards.Add(guid, card);

            card.SetPlayerName($"Player {playerCards.Count}");
            SetPlayerReadyStatus(guid, false);
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