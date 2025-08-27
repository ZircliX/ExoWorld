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
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public override void OnNetworkSpawn(PlayerNetworkController network)
        {
            PlayerNetwork = network;

            if (PlayerManager.HasInstance && PlayerManager.Instance.IsSpawned)
            {
                InitializePlayer();
            }
            else
            {
                Debug.Log("PlayerController waiting for PlayerManager to be ready...");
                PlayerManager.OnInstanceCreated += InitializePlayer;
            }
        }

        private void InitializePlayer()
        {
            if (!PlayerNetwork.IsOwner)
            {
                Destroy(PlayerInput);
                Destroy(Camera.gameObject);
                Destroy(PlayerCamera.gameObject);

                // Safer than destroying Rigidbody
                PlayerMovement.rb.isKinematic = true;
                PlayerMovement.enabled = false;
            }
            else
            {
                CameraManager.Instance.SwitchToCamera(CameraID.PlayerView);
            }
            
            PlayerManager.OnInstanceCreated -= InitializePlayer;
        }

        public override void OnNetworkDespawn()
        {
            if (PlayerNetwork.IsOwner && PlayerManager.HasInstance)
            {
                PlayerManager.Instance.UnregisterPlayer(this);
            }
        }

        public override void OnUpdate()
        {
            if (PlayerNetwork.IsOwner)
            {
                WriteState();
                CheckForReadyStatusChanged();
            }
            else
            {
                ReadState();
            }
        }
        
        private void WriteState()
        {
            Vector3 position = PlayerMovement.Position;
            Quaternion rotation = PlayerMovement.rb.rotation;
                
            PlayerNetworkTransform playerTransform = new PlayerNetworkTransform()
            {
                Position = position,
                Rotation = rotation
            };

            if (PlayerNetwork.IsOwner)
            {
                PlayerNetwork.WritePlayerNetworkTransform(playerTransform);
            }
        }

        private void ReadState()
        {
            Vector3 position = PlayerNetwork.PlayerState.Value.Position;
            transform.position = position;

            Quaternion rotation = PlayerNetwork.PlayerState.Value.Rotation;
            transform.rotation = rotation;
        }

        private void CheckForReadyStatusChanged()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                PlayerNetwork.WritePlayerReadyStatus(!PlayerNetwork.IsReady.Value);
            }
        }
    }
}