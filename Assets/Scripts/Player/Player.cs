using Combat.Weapon;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private Weapon weapon;
        [SerializeField] private Transform orientation;
        private void Update()
        {
            if(!IsOwner) return;
            if (Input.GetMouseButtonDown(0))
            {
                weapon.Shoot(orientation.forward);
                RequestFireServerRpc(orientation.forward);

            }
        }
        
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
        
    }
}