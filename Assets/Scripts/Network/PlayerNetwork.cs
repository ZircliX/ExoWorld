using DeadLink.Player;
using KBCore.Refs;
using Network;
using Unity.Netcode;
using UnityEngine;

namespace Health.Network
{
    public class PlayerNetwork : NetworkBehaviour
    {
        [SerializeField] private bool serverAuth;
        
        [SerializeField, Self] private PlayerController pc;

        private NetworkVariable<PlayerNetworkState> playerState;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        private void Awake()
        {
            NetworkVariableWritePermission perm =
                serverAuth ? NetworkVariableWritePermission.Server : NetworkVariableWritePermission.Owner;
            playerState = new NetworkVariable<PlayerNetworkState>(writePerm: perm);
        }

        public override void OnNetworkSpawn()
        {
            pc.InitializeNetworkSpawn(IsOwner);
        }

        private void Update()
        {
            if (IsOwner)
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
            Vector3 position = pc.PlayerMovement.Position;
            Quaternion rotation = pc.PlayerMovement.rb.rotation;
                
            PlayerNetworkState state = new PlayerNetworkState()
            {
                Position = position,
                Rotation = rotation
            };

            if (!serverAuth || IsServer)
            {
                playerState.Value = state;
            }
            else
            {
                WriteStateServerRpc(state);
            }
        }

        [Rpc(SendTo.Server)]
        private void WriteStateServerRpc(PlayerNetworkState state)
        {
            playerState.Value = state;
        }

        private void ReadState()
        {
            Vector3 position = playerState.Value.Position;
            transform.position = position;

            Quaternion rotation = playerState.Value.Rotation;
            transform.rotation = rotation;
        }
    }
}