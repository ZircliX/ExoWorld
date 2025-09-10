using UnityEngine;

namespace OverBang.GameName.Combat
{
    public class Weapon : MonoBehaviour
    {

        [SerializeField] private Rigidbody bullet;
        [SerializeField] private Transform bulletSpawnPoint;
        
        public void Shoot(Vector3 direction)
        {
            Rigidbody rb = Instantiate(bullet, bulletSpawnPoint.position, Quaternion.identity);
            rb.AddForce(direction * 100, ForceMode.Impulse);
            Destroy(rb.gameObject, 2f);
            
            if (Physics.Raycast(bulletSpawnPoint.position, direction, out RaycastHit hit))
            {
                Debug.DrawRay(bulletSpawnPoint.position, direction * 10f, Color.red);
                
                if (hit.collider.CompareTag("Enemy"))
                {
                    //OnBulletHitRpc(hit.collider.name);
                }
            }
        }
    }
}