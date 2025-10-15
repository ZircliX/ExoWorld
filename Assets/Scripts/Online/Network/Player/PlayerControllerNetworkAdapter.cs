using KBCore.Refs;
using OverBang.GameName.Core.Scenes;
using OverBang.GameName.Gameplay.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Online.Network
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerControllerNetworkAdapter : NetworkBehaviour
    {
        [field: SerializeField, Self] public PlayerController PlayerController { get; private set; }
        
        public NetworkVariable<PlayerNetworkTransform> PlayerState { get; private set; } =
            new NetworkVariable<PlayerNetworkTransform>(writePerm: NetworkVariableWritePermission.Owner);

        public NetworkVariable<ulong> PlayerID { get; private set; } =
            new NetworkVariable<ulong>(writePerm: NetworkVariableWritePermission.Server);

        public NetworkVariable<bool> Ready { get; private set; } =
            new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Server);
        
        public bool IsReady => Ready.Value;

        private readonly float updateRate = 1f / 20f;
        private float timer;

        private void OnValidate() => this.ValidateRefs();

        private void Awake()
        {
            if (PlayerController == null) Debug.LogError($"Player {nameof(PlayerController)} is null.");
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
                    PlayerState.Value.Position,
                    PlayerState.Value.Rotation);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner && PlayerManagerNetworkAdapter.HasInstance)
                PlayerManagerNetworkAdapter.Instance.UnregisterPlayer(this);
        }

        private void FixedUpdate()
        {
            if (!IsOwner || !IsSpawned) return;

            timer += Time.fixedDeltaTime;
            if (timer >= updateRate)
            {
                timer = 0f;
                (Vector3 pos, Quaternion rot) = PlayerController.CaptureState();
                PlayerState.Value = new PlayerNetworkTransform()
                {
                    Position = pos,
                    Rotation = rot
                };
            }
        }

        private void Update()
        {
            if (!IsSpawned) return;

            if (!IsOwner)
            {
                PlayerController.ApplyNetworkState(PlayerState.Value.Position,
                    PlayerState.Value.Rotation);
            }
            else
            {
                HandleInput();
            }
        }

        private void HandleInput()
        {
            if (Keyboard.current.lKey.wasPressedThisFrame)
            {
                //SceneManager.Instance.ChangeScene("Test");
                Debug.Log("Try to change scene to Test");
            }
        }

        public void WritePlayerID(ulong playerID)
        {
            PlayerID.Value = playerID;
        }

        private void WritePlayerReadyStatus(bool readyStatus)
        {
            Ready.Value = readyStatus;
        }

        [Rpc(SendTo.Server)]
        public void RequestSetReadyRpc(bool newReadyStatus)
        {
            WritePlayerReadyStatus(newReadyStatus);
            PlayerManager.Instance.ChangePlayerReadyStatus(PlayerID.Value, newReadyStatus);
        }
        
        public void ToggleReady()
        {
            bool newReady = !Ready.Value;
            RequestSetReadyRpc(newReady);
        }
    }
}