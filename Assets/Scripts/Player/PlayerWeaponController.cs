using OverBang.GameName.Combat;
using OverBang.GameName.Network;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Player
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

        public override void OnUpdate()
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