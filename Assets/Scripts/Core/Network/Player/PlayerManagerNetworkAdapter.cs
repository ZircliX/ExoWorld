using System;
using System.Collections.Generic;
using KBCore.Refs;
using OverBang.GameName.Managers;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Network
{
    [RequireComponent(typeof(PlayerManager))]
    public class PlayerManagerNetworkAdapter : NetworkBehaviour
    {
        public NetworkList<ulong> PlayerIDs { get; private set; }
            = new NetworkList<ulong>(writePerm: NetworkVariableWritePermission.Server);
        
        public Dictionary<ulong, PlayerControllerNetworkAdapter> Players { get; private set; }

        [field: SerializeField, Self] public PlayerManager PlayerManager { get; private set; }
        
        public static event Action OnInstanceCreated;

        public static PlayerManagerNetworkAdapter Instance { get; private set; }
        public static bool HasInstance => Instance != null;

        private void OnValidate() => this.ValidateRefs();

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                PlayerIDs.Clear();
            }

            SetupSingleton();
        }
        
        private void SetupSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("PlayerManagerNetworkAdapter Instance already created");
                Destroy(gameObject);
                return;
            }

            Players = new Dictionary<ulong, PlayerControllerNetworkAdapter>(4);
            Instance = this;
            Debug.Log("PlayerManagerNetworkAdapter Instance created");
            OnInstanceCreated?.Invoke();
        }

        public void RegisterPlayer(PlayerControllerNetworkAdapter playerController)
        {
            if (IsServer)
            {
                RegisterPlayerServer(playerController);
            }
            else
            {
                ulong playerID = playerController.PlayerControllerNetwork.PlayerID.Value;
                RegisterPlayerRpc(playerID);
            }
        }

        private void RegisterPlayerServer(PlayerControllerNetworkAdapter playerController)
        {
            ulong playerID = playerController.PlayerControllerNetwork.PlayerID.Value;
            if (PlayerIDs.Contains(playerID)) return;

            Debug.Log($"[PlayerManagerNetworkAdapter] RegisterPlayerServer");
            playerController.PlayerControllerNetwork.WritePlayerID(playerID);
            PlayerIDs.Add(playerID);
            Players.Add(playerID, playerController);
            PlayerManager.RegisterPlayer(playerID, playerController.PlayerController);
        }

        [Rpc(SendTo.Server)]
        private void RegisterPlayerRpc(ulong clientId)
        {
            NetworkObject obj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            PlayerControllerNetworkAdapter playerController = obj.GetComponent<PlayerControllerNetworkAdapter>();
            if (playerController != null)
                RegisterPlayerServer(playerController);
        }

        public void UnregisterPlayer(PlayerControllerNetworkAdapter playerController)
        {
            if (IsServer)
            {
                UnregisterPlayerServer(playerController);
            }
            else
            {
                ulong playerID = playerController.PlayerControllerNetwork.PlayerID.Value;
                UnregisterPlayerRpc(playerID);
            }
        }

        private void UnregisterPlayerServer(PlayerControllerNetworkAdapter playerController)
        {
            ulong playerID = playerController.PlayerControllerNetwork.PlayerID.Value;
            if (!PlayerIDs.Contains(playerID)) return;
            
            PlayerIDs.Remove(playerID);
            Players.Remove(playerID);
            PlayerManager.UnregisterPlayer(playerID);
        }

        [Rpc(SendTo.Server)]
        private void UnregisterPlayerRpc(ulong clientId)
        {
            NetworkObject obj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            PlayerControllerNetworkAdapter playerController = obj.GetComponent<PlayerControllerNetworkAdapter>();
            if (playerController != null)
                UnregisterPlayerServer(playerController);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void TeleportPlayersRpc(Vector3 position)
        {
            foreach (ulong id in PlayerIDs)
            {
                PlayerManager.TeleportPlayer(id, position);
            }
        }
    }
}