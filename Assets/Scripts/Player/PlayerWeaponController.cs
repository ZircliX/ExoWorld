using Combat.Weapon;
using Health.Network;
using Network.Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace DeadLink.Player
{
    public class PlayerWeaponController : NetworkChildren
    {
        [SerializeField] private Weapon weapon;
        [SerializeField] private Transform orientation;
        private PlayerNetworkController playerNetwork;
        
        public override void OnNetworkSpawn(PlayerNetworkController network)
        {
            playerNetwork = network;
        }

        public override void OnNetworkDespawn()
        {
        }

        public override void OnNetworkUpdate()
        {
            if (playerNetwork.IsOwner)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    weapon.Shoot(orientation.forward);
                    RequestFireServerRpc(orientation.forward);
                }
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
            if (playerNetwork.IsOwner) return;
            weapon.Shoot(direction);
        }
    }
}