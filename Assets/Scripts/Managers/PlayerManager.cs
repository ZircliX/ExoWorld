using System;
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
        public NetworkList<byte> Players { get; private set; }
            = new NetworkList<byte>(writePerm: NetworkVariableWritePermission.Server);
        
        public event Action<byte> OnPlayerRegistered;
        public event Action<byte> OnPlayerUnregistered;
        public event Action<byte, bool> OnPlayerReadyStatusChanged;

        public static event Action OnInstanceCreated;
        
        public static PlayerManager Instance { get; private set; }
        public static bool HasInstance => Instance != null;

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

        public void RegisterPlayer(PlayerNetworkController playerController)
        {
            if (IsSpawned)
            {
                RegisterPlayer(playerController.PlayerID.Value);
                playerController.WritePlayerID((byte) Players.Count);
            }
            else
            {
                Debug.LogError($"Player Manager is not spawned. Cannot register player {playerController.PlayerID}.");
            }
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
            if (IsSpawned)
            {
                UnregisterPlayer(playerController.PlayerNetwork.PlayerID.Value);
            }
            else
            {
                Debug.LogError($"Player Manager is not spawned. Cannot unregister player {playerController.PlayerNetwork.PlayerID}.");
            }
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
            if (IsSpawned)
            {
                ChangePlayerReadyStatus(playerController.PlayerNetwork.PlayerID.Value, playerController.PlayerNetwork.IsReady.Value);
            }
            else
            {
                Debug.LogError($"Player Manager is not spawned. Cannot unregister player {playerController.PlayerNetwork.PlayerID}.");
            }
        }

        public void ChangePlayerReadyStatus(byte playerID, bool readyStatus)
        {
            if (IsSpawned)
            {
                OnPlayerReadyStatusChanged?.Invoke(playerID, readyStatus);
            }
            else
            {
                Debug.LogError($"Player Manager is not spawned. Cannot change player's ready status {playerID}.");
            }
        }
    }
}