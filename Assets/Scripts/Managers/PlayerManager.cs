using System;
using System.Collections.Generic;
using OverBang.GameName.Network;
using OverBang.GameName.Network.Static;
using OverBang.GameName.Player;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Managers
{
    public class PlayerManager : NetworkBehaviour
    {
        public Dictionary<byte, PlayerController> PlayerControllers { get; private set; }
        public NetworkList<byte> Players { get; private set; }
            = new NetworkList<byte>(writePerm: NetworkVariableWritePermission.Server);
        
        public event Action<byte> OnPlayerRegistered;
        public event Action<byte> OnPlayerUnregistered;
        public event Action<byte, bool> OnPlayerReadyStatusChanged;

        public static event Action OnInstanceCreated;
        
        public static PlayerManager Instance { get; private set; }
        public static bool HasInstance => Instance != null;

        private void Awake()
        {
            PlayerControllers = new Dictionary<byte, PlayerController>(4);
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

            if (IsServer)
            {
                Debug.Log($"[Server] PlayerManager spawned. Instance set to {this}");
            }
            else
            {
                Debug.Log($"[Client] PlayerManager spawned. Instance set to {this}");
            }

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

            playerController.PlayerNetwork.WritePlayerID((byte)(Players.Count + 1));
            RegisterPlayer(playerController.PlayerNetwork.PlayerID.Value);

            PlayerControllers.Add(playerController.PlayerNetwork.PlayerID.Value, playerController);
        }
        
        public void RegisterPlayer(byte playerID)
        {
            if (!this.CanRunNetworkOperation()) return;
            
            if (IsServer)
            {
                // Direct server-side cleanup
                RegisterPlayerInternal(playerID);
            }
            else
            {
                // Ask the server to do it
                RegisterPlayerRpc(playerID);
            }
        }
        
        private void RegisterPlayerInternal(byte playerID)
        {
            if (Players.Contains(playerID)) return;

            Players.Add(playerID);
            OnPlayerRegistered?.Invoke(playerID);
        }
        
        [Rpc(SendTo.Server)]
        private void RegisterPlayerRpc(byte playerID)
        {
            Debug.Log($"[Server] Registering player {playerID}.");
            // Only runs on the server
            RegisterPlayerInternal(playerID);
        }
        
        public void UnregisterPlayer(PlayerController playerController)
        {
            if (!IsSpawned)
            {
                Debug.LogError(
                    $"Player Manager is not spawned. Cannot unregister player {playerController.PlayerNetwork.PlayerID}.");
                return;
            }

            UnregisterPlayer(playerController.PlayerNetwork.PlayerID.Value);
        }
        
        public void UnregisterPlayer(byte playerID)
        {
            if (!this.CanRunNetworkOperation()) return;
            
            if (IsServer)
            {
                // Direct server-side cleanup
                UnregisterPlayerInternal(playerID);
            }
            else
            {
                // Ask the server to do it
                UnregisterPlayerRpc(playerID);
            }
        }
        
        private void UnregisterPlayerInternal(byte playerID)
        {
            if (!Players.Contains(playerID)) return;

            Players.Remove(playerID);
            OnPlayerUnregistered?.Invoke(playerID);
        }
        
        [Rpc(SendTo.Server)]
        private void UnregisterPlayerRpc(byte playerID)
        {
            // Only runs on the server
            UnregisterPlayerInternal(playerID);
        }
        
        public void ChangePlayerReadyStatus(PlayerController playerController)
        {
            if (!IsSpawned)
            {
                Debug.LogError(
                    $"Player Manager is not spawned. Cannot unregister player {playerController.PlayerNetwork.PlayerID}.");
                return;
            }

            ChangePlayerReadyStatus(playerController.PlayerNetwork.PlayerID.Value,
                playerController.PlayerNetwork.IsReady.Value);
        }

        public void ChangePlayerReadyStatus(byte playerID, bool readyStatus)
        {
            if (!IsSpawned)
            {
                Debug.LogError($"Player Manager is not spawned. Cannot change player's ready status {playerID}.");
                return;
            }

            OnPlayerReadyStatusChanged?.Invoke(playerID, readyStatus);
        }
    }
}