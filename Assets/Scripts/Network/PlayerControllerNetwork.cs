using Combat.Weapon;
using DeadLink.Player;
using KBCore.Refs;
using Network;
using Unity.Netcode;
using UnityEngine;

namespace Health.Network
{
    public class PlayerControllerNetwork : NetworkBehaviour
    {
        [SerializeField] private bool serverAuth;
        
        [SerializeField, Self] private PlayerController pc;

        private NetworkVariable<PlayerNetworkState> playerState;
        
        [SerializeField] private Weapon weapon;
        [SerializeField] private Transform orientation;

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
            
            if(!IsOwner) return;
            if (Input.GetMouseButtonDown(0))
            {
                weapon.Shoot(orientation.forward);
                RequestFireServerRpc(orientation.forward);

            }
        }
        
        #region Shoot
        [Rpc(SendTo.Everyone)]
        private void OnBulletHitRpc(string hit)
        {
            /*
            Debug.Log($"Name : {hit}");
            Debug.Log("Lois est trop beau whallah");
            */
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
            weapon.Shoot(direction);
            
        }
        #endregion
        
        #region States
        private void WriteState()
        {
            Vector3 position = pc.PlayerMovement.Position;
            Quaternion rotation = pc.PlayerMovement.rb.rotation;
                
            PlayerNetworkState state = new PlayerNetworkState()
            {
                Position = position,
                Rotation = rotation
            };

            // Soit HOST soit SERVER
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
        #endregion
    }
}
