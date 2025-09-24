using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Player
{
    public class PlayerManager : NetworkBehaviour
    {
        public Dictionary<ulong, PlayerController> Players { get; private set; }

        public event Action<ulong> OnPlayerRegistered;
        public event Action<ulong> OnPlayerUnregistered;
        public event Action<ulong, bool> OnPlayerReadyStatusChanged;

        public static event Action OnInstanceCreated;

        public static PlayerManager Instance { get; private set; }
        public static bool HasInstance => Instance != null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Players = new Dictionary<ulong, PlayerController>(4);
            OnInstanceCreated?.Invoke();
        }

        public void RegisterPlayer(ulong playerId, PlayerController controller)
        {
            if (!Players.TryAdd(playerId, controller)) return;
            OnPlayerRegistered?.Invoke(playerId);
        }

        public void UnregisterPlayer(ulong playerId)
        {
            if (!Players.Remove(playerId)) return;
            OnPlayerUnregistered?.Invoke(playerId);
        }

        public void ChangePlayerReadyStatus(ulong playerId, bool readyStatus)
        {
            OnPlayerReadyStatusChanged?.Invoke(playerId, readyStatus);
        }

        public void TeleportPlayer(ulong playerId, Vector3 position)
        {
            if (!Players.TryGetValue(playerId, out PlayerController player)) return;

            player.PlayerMovement.Rb.MovePosition(position);
        }
    }
}