using Unity.Netcode;
using UnityEngine;

namespace Combat.Weapon
{
    public class Weapon : NetworkBehaviour
    {

        [SerializeField] private Rigidbody bullet;
        [SerializeField] private Transform bulletSpawnPoint;
        
        public void Shoot(Vector3 direction)
        {
            RequestFireServerRpc(direction);
            Rigidbody rb = Instantiate(bullet, bulletSpawnPoint.position, Quaternion.identity);
            rb.AddForce(direction * 100, ForceMode.Impulse);
            Destroy(rb.gameObject, 2f);
            
            if (Physics.Raycast(bulletSpawnPoint.position, direction, out RaycastHit hit))
            {
                Debug.DrawRay(bulletSpawnPoint.position, direction * 10f, Color.red);
                
                if (hit.collider.CompareTag("Enemy"))
                {
                    OnBulletHitRpc(hit.collider.name);
                }
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
            Shoot(direction);
            
        }
    }
}