using OverBang.Pooling;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class WeaponImplementation : Weapon
    {
        [SerializeField] private Transform shootPoint;

        public override void Fire()
        {
            GameObject bulletObject = WeaponData.BulletData.BulletPoolResource.Spawn<GameObject>();
            
            if (bulletObject == null)
            {
                Debug.LogWarning($"Could not get Bullet from pool.");
                return;
            }
            
            if (bulletObject.TryGetComponent(out BulletImplementation bullet))
            {
                Vector3 dir = this.GetBulletDirection(shootPoint) + playerCamera.transform.forward;
                bullet.Fire(shootPoint, dir, WeaponData.BulletData);
            }
            else
            {
                Debug.LogWarning($"Could not get component {nameof(BulletImplementation)} from bullet.", bullet);
            }
        }
    }
}