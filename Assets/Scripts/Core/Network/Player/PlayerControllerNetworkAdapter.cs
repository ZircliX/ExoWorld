using KBCore.Refs;
using OverBang.GameName.Core.Scene;
using OverBang.GameName.Managers;
using OverBang.GameName.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Network
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(PlayerNetworkController))]
    public class PlayerControllerNetworkAdapter : NetworkBehaviour
    {
        [field: SerializeField, Self] public PlayerController PlayerController { get; private set; }
        [field: SerializeField, Self] public PlayerNetworkController PlayerNetworkController { get; private set; }

        private readonly float updateRate = 1f / 20f;
        private float timer;

        private void OnValidate() => this.ValidateRefs();

        private void Awake()
        {
            if (PlayerController == null) Debug.LogError($"Player {nameof(PlayerController)} is null.");
            if (PlayerNetworkController == null) Debug.LogError($"PlayerNetworkController is null.");
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                PlayerController.EnableLocalControls();
            }
            else
            {
                PlayerController.DisableRemoteControls(
                    PlayerNetworkController.PlayerState.Value.Position,
                    PlayerNetworkController.PlayerState.Value.Rotation);
            }

            if (PlayerManagerNetworkAdapter.HasInstance && PlayerManagerNetworkAdapter.Instance.IsSpawned)
            {
                InitializePlayer();
            }
            else
            {
                Debug.Log("Waiting for PlayerManagerNetworkAdapter to spawn");
                PlayerManagerNetworkAdapter.OnInstanceCreated += InitializePlayer;
            }
        }

        private void InitializePlayer()
        {
            Debug.Log($"[PlayerNetworkAdapter] InitializePlayer");
            PlayerManagerNetworkAdapter.Instance.RegisterPlayer(this);
            PlayerManager.OnInstanceCreated -= InitializePlayer;
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner && PlayerManagerNetworkAdapter.HasInstance)
                PlayerManagerNetworkAdapter.Instance.UnregisterPlayer(this);
        }

        private void FixedUpdate()
        {
            if (!IsOwner || !PlayerNetworkController.IsSpawned) return;

            timer += Time.fixedDeltaTime;
            if (timer >= updateRate)
            {
                timer = 0f;
                (Vector3 pos, Quaternion rot) = PlayerController.CaptureState();
                PlayerNetworkController.PlayerState.Value = new PlayerNetworkTransform()
                {
                    Position = pos,
                    Rotation = rot
                };
            }
        }

        private void Update()
        {
            if (!PlayerNetworkController.IsSpawned) return;

            if (!IsOwner)
            {
                PlayerController.ApplyNetworkState(PlayerNetworkController.PlayerState.Value.Position,
                    PlayerNetworkController.PlayerState.Value.Rotation);
            }
            else
            {
                HandleInput();
            }
        }

        private void HandleInput()
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                bool newReady = !PlayerNetworkController.IsReady.Value;
                PlayerNetworkController.RequestSetReadyRpc(newReady);
            }

            if (Keyboard.current.lKey.wasPressedThisFrame)
            {
                SceneManager.Instance.ChangeScene("Test");
                Debug.Log("Try to change scene to Test");
            }
            
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector3 direction = PlayerController.Camera.transform.forward;
                PlayerController.Weapon.Shoot(direction);
                RequestFireServerRpc(direction);
            }
        }
        
        [Rpc(SendTo.Server)]
        private void RequestFireServerRpc(Vector3 direction)
        {
            FireClientRpc(direction);
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void FireClientRpc(Vector3 direction)
        {
            if (IsOwner) return;
            PlayerController.Weapon.Shoot(direction);
        }
    }
}