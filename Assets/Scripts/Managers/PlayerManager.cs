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

            if (IsServer)
            {
                WritePlayerIDInternal(playerController);
            }
            else
            {
                WritePlayerIDRpc(NetworkManager.Singleton.LocalClientId);
            }
        }

        private void WritePlayerIDInternal(PlayerController player)
        {
            byte newID = (byte)(Players.Count + 1);
            player.PlayerNetwork.WritePlayerID(newID);
            
            RegisterPlayerInternal(newID, player);
        }
        
        private void RegisterPlayerInternal(byte playerID, PlayerController player)
        {
            if (Players.Contains(playerID)) return;

            Players.Add(playerID);
            PlayerControllers.Add(playerID, player);
            
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

            WritePlayerIDInternal(playerController);
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
        
        private void UnregisterPlayerInternal(byte playerID)
        {
            if (!Players.Contains(playerID)) return;

            Players.Remove(playerID);
            PlayerControllers.Remove(playerID);
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

        public void ChangePlayerReadyStatus(byte playerID, bool readyStatus)
        {
            StartCoroutine(this.CanRunNetworkOperation(() =>
            {
                Debug.Log($"Player {playerID} to {readyStatus}");
                OnPlayerReadyStatusChanged?.Invoke(playerID, readyStatus);
            }));
        }
    }
}