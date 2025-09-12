using System.Collections.Generic;
using KBCore.Refs;
using OverBang.GameName.Managers;
using OverBang.GameName.Network;
using OverBang.GameName.Player;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.HUB
{
    [RequireComponent(typeof(HubController))]
    public class HubControllerNetworkAdapter : NetworkBehaviour, ICheckForGameStart
    {
        [field: SerializeField, Self] public HubController Hub { get; private set; }

        public NetworkList<PlayerHubState> PlayerStates { get; private set; }
            = new NetworkList<PlayerHubState>(writePerm: NetworkVariableWritePermission.Server);

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public override void OnNetworkSpawn()
        {
            PlayerStates.OnListChanged += OnPlayersListChanged;

            if (IsServer)
            {
                Hub.OnHubPlayerAdded += HandlePlayerAdded;
                Hub.OnHubPlayerRemoved += HandlePlayerRemoved;
                Hub.OnHubPlayerReadyChanged += HandlePlayerReadyChanged;
                
                PlayerStates.Clear();
                foreach (KeyValuePair<ulong, PlayerController> kvp in PlayerManager.Instance.Players)
                {
                    PlayerStates.Add(new PlayerHubState(kvp.Key, false));
                }
            }
        }

        public override void OnNetworkDespawn()
        {
            PlayerStates.OnListChanged -= OnPlayersListChanged;
            
            if (IsServer)
            {
                Hub.OnHubPlayerAdded -= HandlePlayerAdded;
                Hub.OnHubPlayerRemoved -= HandlePlayerRemoved;
                Hub.OnHubPlayerReadyChanged -= HandlePlayerReadyChanged;
            }
        }
        
        private void HandlePlayerAdded(ulong playerId)
            => PlayerStates.Add(new PlayerHubState(playerId, false));

        private void HandlePlayerRemoved(ulong playerId)
        {
            for (int i = 0; i < PlayerStates.Count; i++)
            {
                if (PlayerStates[i].PlayerID == playerId)
                {
                    PlayerStates.RemoveAt(i);
                    break;
                }
            }
        }

        private void HandlePlayerReadyChanged(ulong playerId, bool isReady)
        {
            for (int i = 0; i < PlayerStates.Count; i++)
            {
                if (PlayerStates[i].PlayerID == playerId)
                {
                    PlayerStates[i] = new PlayerHubState(playerId, isReady);
                    break;
                }
            }
        }

        private void OnPlayersListChanged(NetworkListEvent<PlayerHubState> change)
        {
            switch (change.Type)
            {
                case NetworkListEvent<PlayerHubState>.EventType.Add:
                    Hub.AddPlayerCard(change.Value.PlayerID, change.Value.IsReady);
                    break;
                case NetworkListEvent<PlayerHubState>.EventType.Remove:
                    Hub.RemovePlayerCard(change.Value.PlayerID);
                    break;
                case NetworkListEvent<PlayerHubState>.EventType.Value:
                    CheckForGameStartInternal();
                    Hub.UpdatePlayerCard(change.Value.PlayerID, change.Value.IsReady);
                    break;
                case NetworkListEvent<PlayerHubState>.EventType.Clear:
                    Hub.ClearPlayerCards();
                    break;
            }
        }
        
        private void CheckForGameStartInternal()
        {
            if (!IsServer) return;
            if (PlayerStates.Count == 0) return;

            foreach (PlayerHubState player in PlayerStates)
            {
                if (!player.IsReady) return;
            }

            Debug.LogWarning("[Hub] All players ready -> starting game!");
            StartGameClientRpc();
        }

        public void StartGame()
        {
            Hub.StartGame();
            PlayerManager.Instance.GetComponent<PlayerManagerNetworkAdapter>()
                .TeleportPlayersRpc(Hub.transform.position);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void StartGameClientRpc()
        {
            StartGame();
        }
    }
}