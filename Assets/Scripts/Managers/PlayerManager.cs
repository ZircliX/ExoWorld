using System;
using System.Collections.Generic;
using KBCore.Refs;
using OverBang.GameName.Network;
using OverBang.GameName.Player;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Managers
{
    public class PlayerManager : NetworkBehaviour
    {
        public Dictionary<ulong, PlayerController> Players { get; private set; }
        public NetworkList<ulong> PlayerIDs { get; private set; }
            = new NetworkList<ulong>(writePerm: NetworkVariableWritePermission.Server);
        
        [field: SerializeField, Self] public PingManager PingManager { get; private set; }
        
        public event Action<ulong> OnPlayerRegistered;
        public event Action<ulong> OnPlayerUnregistered;
        public event Action<ulong> OnPlayerReadyStatusChanged;

        public static event Action OnInstanceCreated;
        
        public static PlayerManager Instance { get; private set; }
        public static bool HasInstance => Instance != null;

        private void OnValidate() => this.ValidateRefs();

        private void Awake()
        {
            Players = new Dictionary<ulong, PlayerController>(4);
        }

        public override void OnNetworkSpawn()
        {
            // Enforce singleton on all instances (server & clients)
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Multiple PlayerManager instances detected. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            Instance = this;

            Debug.Log(IsServer
                ? $"[Server] PlayerManager spawned. Instance set to {this}"
                : $"[Client] PlayerManager spawned. Instance set to {this}");

            OnInstanceCreated?.Invoke();
        }

        public void RegisterPlayer(PlayerController playerController)
        {
            if (!IsSpawned)
            {
                Debug.LogError(
                    $"Player Manager is not spawned. Cannot register player {playerController.PlayerNetwork.PlayerID}.");
                return;
            }

            ulong clientID = NetworkManager.Singleton.LocalClientId;

            if (IsServer)
            {
                WritePlayerIDInternal(playerController, clientID);
            }
            else
            {
                WritePlayerIDRpc(clientID);
            }
        }

        private void WritePlayerIDInternal(PlayerController player, ulong clientID)
        {
            player.PlayerNetwork.WritePlayerID(clientID);
            
            RegisterPlayerInternal(clientID, player);
        }
        
        private void RegisterPlayerInternal(ulong playerID, PlayerController player)
        {
            if (PlayerIDs.Contains(playerID)) return;

            PlayerIDs.Add(playerID);
            Players.Add(playerID, player);
            
            Debug.Log($"[Server] Registered player {playerID}");
            OnPlayerRegistered?.Invoke(playerID);
        }

        [Rpc(SendTo.Server)]
        private void WritePlayerIDRpc(ulong requestingClientId)
        {
            // Only the server runs this
            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(requestingClientId, 
                    out NetworkClient client))
                return;

            PlayerController playerController = 
                client.PlayerObject.GetComponent<PlayerController>();
            if (playerController == null) return;

            WritePlayerIDInternal(playerController, requestingClientId);
        }
        
        public void UnregisterPlayer(PlayerController playerController)
        {
            if (!IsSpawned)
            {
                Debug.LogError(
                    $"Player Manager is not spawned. Cannot unregister player {playerController.PlayerNetwork.PlayerID}.");
                return;
            }

            if (IsServer)
            {
                UnregisterPlayerInternal(playerController.PlayerNetwork.PlayerID.Value);
            }
            else
            {
                UnregisterPlayerRpc(NetworkManager.Singleton.LocalClientId);
            }
        }
        
        private void UnregisterPlayerInternal(ulong playerID)
        {
            if (!PlayerIDs.Contains(playerID)) return;

            PlayerIDs.Remove(playerID);
            Players.Remove(playerID);
            OnPlayerUnregistered?.Invoke(playerID);
        }
        
        [Rpc(SendTo.Server)]
        private void UnregisterPlayerRpc(ulong requestingClientId)
        {
            // Only the server runs this
            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(requestingClientId, 
                    out NetworkClient client))
                return;

            PlayerController playerController = 
                client.PlayerObject.GetComponent<PlayerController>();
            if (playerController == null) return;

            UnregisterPlayerInternal(playerController.PlayerNetwork.PlayerID.Value);
        }

        public void ChangePlayerReadyStatus(ulong playerID, bool readyStatus)
        {
            OnPlayerReadyStatusChanged?.Invoke(playerID);
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        public void TeleportPlayersRpc(Vector3 position)
        {
            foreach (PlayerController playerController in Players.Values)
            {
                TeleportPlayer(playerController, position);
            }
        }

        public void TeleportPlayer(PlayerController player, Vector3 position)
        {
            player.PlayerMovement.Rb.MovePosition(position);

            PlayerNetworkTransform newPos = new PlayerNetworkTransform()
            {
                Position = position,
                Rotation = player.PlayerMovement.Rb.rotation,
            };
            player.PlayerNetwork.WritePlayerNetworkTransformRpc(newPos);
        }
    }
}