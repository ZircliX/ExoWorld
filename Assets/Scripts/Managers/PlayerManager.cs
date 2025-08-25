using System;
using OverBang.GameName.Network.Static;
using OverBang.GameName.Player;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Managers
{
    public class PlayerManager : NetworkBehaviour
    {
        public NetworkList<FixedString64Bytes> Players { get; private set; }
            = new NetworkList<FixedString64Bytes>(writePerm: NetworkVariableWritePermission.Server);
        
        public event Action<string> OnPlayerRegistered;
        public event Action<string> OnPlayerUnregistered;
        public event Action<string, bool> OnPlayerReadyStatusChanged;

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

        public void RegisterPlayer(PlayerController playerController)
        {
            if (IsSpawned)
            {
                RegisterPlayer(playerController.Guid);
            }
            else
            {
                Debug.LogError($"Player Manager is not spawned. Cannot register player {playerController.Guid}.");
            }
        }
        
        public void RegisterPlayer(string playerGuid)
        {
            if (!this.CanRunNetworkOperation()) return;
            
            if (IsServer)
            {
                // Direct server-side cleanup
                RegisterPlayerInternal(playerGuid);
            }
            else
            {
                // Ask the server to do it
                RegisterPlayerRpc(playerGuid);
            }
        }
        
        [Rpc(SendTo.Server)]
        private void RegisterPlayerRpc(string playerGuid)
        {
            Debug.Log($"[Server] Registering player {playerGuid}.");
            // Only runs on the server
            RegisterPlayerInternal(playerGuid);
        }

        private void RegisterPlayerInternal(string playerGuid)
        {
            FixedString64Bytes fixedGuid = new FixedString64Bytes(playerGuid);
            
            if (Players.Contains(fixedGuid)) return;

            Players.Add(fixedGuid);
            OnPlayerRegistered?.Invoke(playerGuid);
        }
        
        public void UnregisterPlayer(PlayerController playerController)
        {
            if (IsSpawned)
            {
                UnregisterPlayer(playerController.Guid);
            }
            else
            {
                Debug.LogError($"Player Manager is not spawned. Cannot unregister player {playerController.Guid}.");
            }
        }
        
        public void UnregisterPlayer(string playerGuid)
        {
            if (!this.CanRunNetworkOperation()) return;
            
            if (IsServer)
            {
                // Direct server-side cleanup
                UnregisterPlayerInternal(playerGuid);
            }
            else
            {
                // Ask the server to do it
                UnregisterPlayerRpc(playerGuid);
            }
        }
        
        [Rpc(SendTo.Server)]
        private void UnregisterPlayerRpc(string playerGuid)
        {
            // Only runs on the server
            UnregisterPlayerInternal(playerGuid);
        }
        
        private void UnregisterPlayerInternal(string playerGuid)
        {
            FixedString64Bytes guid = new FixedString64Bytes(playerGuid);

            if (!Players.Contains(guid)) return;

            Players.Remove(guid);
            OnPlayerUnregistered?.Invoke(playerGuid);
        }

        public void ChangePlayerReadyStatus(PlayerController playerController)
        {
            if (IsSpawned)
            {
                ChangePlayerReadyStatus(playerController.Guid, playerController.PlayerNetwork.IsReady.Value);
            }
            else
            {
                Debug.LogError($"Player Manager is not spawned. Cannot unregister player {playerController.Guid}.");
            }
        }

        public void ChangePlayerReadyStatus(string playerGuid, bool readyStatus)
        {
            if (IsSpawned)
            {
                OnPlayerReadyStatusChanged?.Invoke(playerGuid, readyStatus);
            }
            else
            {
                Debug.LogError($"Player Manager is not spawned. Cannot change player's ready status {playerGuid}.");
            }
        }
    }
}