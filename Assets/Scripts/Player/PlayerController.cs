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

        public string Guid { get; private set; }
        public PlayerNetworkController PlayerNetwork { get; private set; }
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public override void OnNetworkSpawn(PlayerNetworkController network)
        {
            PlayerNetwork = network;
            
            if (!network.IsOwner)
            {
                Destroy(PlayerInput);
                Destroy(Camera.gameObject);
                Destroy(PlayerCamera.gameObject);
                
                Destroy(PlayerMovement.rb);
                Destroy(PlayerMovement);
            }
            else
            {
                Guid = System.Guid.NewGuid().ToString();
                PlayerManager.Instance.RegisterPlayer(this);
                CameraManager.Instance.SwitchToCamera(CameraID.PlayerView);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (PlayerNetwork.IsOwner)
            {
                PlayerManager.Instance.UnregisterPlayer(this);
            }
        }

        public override void OnUpdate()
        {
            if (PlayerNetwork.IsOwner)
            {
                WriteState();
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
                
            PlayerNetworkState state = new PlayerNetworkState()
            {
                Position = position,
                Rotation = rotation
            };

            if (PlayerNetwork.IsServer)
            {
                PlayerNetwork.WritePlayerState(state);
            }
        }

        private void ReadState()
        {
            Vector3 position = PlayerNetwork.PlayerState.Value.Position;
            transform.position = position;

            Quaternion rotation = PlayerNetwork.PlayerState.Value.Rotation;
            transform.rotation = rotation;
        }
    }
}