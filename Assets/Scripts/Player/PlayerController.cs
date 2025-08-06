using KBCore.Refs;
using OverBang.GameName.Cameras;
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

        private PlayerNetworkController playerNetwork;
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public override void OnNetworkSpawn(PlayerNetworkController network)
        {
            playerNetwork = network;
            
            if (!network.IsOwner)
            {
                Destroy(PlayerInput);
                Destroy(Camera.gameObject);
                Destroy(PlayerCamera.gameObject);
                
                Destroy(PlayerMovement.rb);
                Destroy(PlayerMovement);
            }
        }

        public override void OnUpdate()
        {
            if (playerNetwork.IsOwner)
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

            if (playerNetwork.IsServer)
            {
                playerNetwork.WritePlayerState(state);
            }
        }

        private void ReadState()
        {
            Vector3 position = playerNetwork.PlayerState.Value.Position;
            transform.position = position;

            Quaternion rotation = playerNetwork.PlayerState.Value.Rotation;
            transform.rotation = rotation;
        }
    }
}