using KBCore.Refs;
using OverBang.GameName.Cameras;
using OverBang.GameName.Managers;
using OverBang.GameName.Metrics;
using OverBang.GameName.Movement;
using OverBang.GameName.Network;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Player
{
    public class PlayerController : NetworkChildren
    {
        [field: SerializeField, Self] public PlayerInput PlayerInput { get; private set; }
        [field: SerializeField, Child] public PlayerMovement PlayerMovement { get; private set; }
        [field: SerializeField, Child] public PlayerCamera PlayerCamera { get; private set; }
        [field: SerializeField, Child] public Camera Camera { get; private set; }

        public PlayerNetworkController PlayerNetwork { get; private set; }

        private float updateRate = 1f / 20f; // 20Hz network send
        private float timer;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public override void OnNetworkSpawn(PlayerNetworkController network)
        {
            PlayerNetwork = network;

            if (!PlayerNetwork.IsOwner)
            {
                // Non-owner: strip controls & camera
                Destroy(PlayerInput);
                Destroy(Camera.gameObject);
                Destroy(PlayerCamera.gameObject);

                // Keep Rigidbody but disable local movement
                PlayerMovement.Rb.isKinematic = true;
                PlayerMovement.Rb.interpolation = RigidbodyInterpolation.Interpolate;
                PlayerMovement.enabled = false;

                // Snap to the latest state immediately
                Vector3 pos = PlayerNetwork.PlayerState.Value.Position;
                Quaternion rot = PlayerNetwork.PlayerState.Value.Rotation;
                PlayerMovement.Rb.position = pos;
                PlayerMovement.Rb.rotation = rot;
            }
            else
            {
                CameraManager.Instance.SwitchToCamera(CameraID.PlayerView);
            }

            if (PlayerManager.HasInstance && PlayerManager.Instance.IsSpawned)
            {
                InitializePlayer();
            }
            else
            {
                PlayerManager.OnInstanceCreated += InitializePlayer;
            }
        }

        private void InitializePlayer()
        {
            PlayerManager.Instance.RegisterPlayer(this);
            PlayerManager.OnInstanceCreated -= InitializePlayer;
        }

        public override void OnNetworkDespawn()
        {
            if (PlayerNetwork.IsOwner && PlayerManager.HasInstance)
            {
                PlayerManager.Instance.UnregisterPlayer(this);
            }
        }

        private void FixedUpdate()
        {
            if (PlayerNetwork == null) return;

            if (PlayerNetwork.IsOwner)
            {
                WriteState();
            }
        }

        public override void OnUpdate()
        {
            if (PlayerNetwork == null) return;

            if (!PlayerNetwork.IsOwner)
            {
                ReadState();
            }
            else
            {
                CheckForReadyStatusChanged();
            }
        }

        private void WriteState()
        {
            timer += Time.fixedDeltaTime;
            if (timer < updateRate) return;
            timer = 0f;

            PlayerNetwork.PlayerState.Value = new PlayerNetworkTransform()
            {
                Position = PlayerMovement.Rb.position,
                Rotation = PlayerMovement.Rb.rotation
            };
        }

        private void ReadState()
        {
            Vector3 targetPos = PlayerNetwork.PlayerState.Value.Position;
            Quaternion targetRot = PlayerNetwork.PlayerState.Value.Rotation;

            Vector3 currentPos = PlayerMovement.Rb.position;
            Quaternion currentRot = PlayerMovement.Rb.rotation;

            // Smooth interpolation (factor-based, not delta-time scaled)
            float lerpFactor = 6f;
            PlayerMovement.Rb.MovePosition(Vector3.Lerp(currentPos, targetPos, lerpFactor));
            PlayerMovement.Rb.MoveRotation(Quaternion.Slerp(currentRot, targetRot, lerpFactor));
        }

        private void CheckForReadyStatusChanged()
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                bool newReadyStatus = !PlayerNetwork.IsReady.Value;
                PlayerNetwork.RequestSetReadyRpc(newReadyStatus);
            }
        }
    }
}