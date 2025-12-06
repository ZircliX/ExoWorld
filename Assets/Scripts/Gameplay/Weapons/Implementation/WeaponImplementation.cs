using OverBang.Pooling;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class WeaponImplementation : Weapon
    {
        public override void Fire()
        {
            Vector3 dir = this.GetBulletDirection(Rig.shootPoint) + playerCamera.transform.forward;
            FireWithDirection(Rig.shootPoint.position, dir);
        }

        public override void FireWithDirection(Vector3 origin, Vector3 direction)
        {
            GameObject bulletObject = WeaponData.BulletData.BulletPoolResource.Spawn<GameObject>();
            
            if (bulletObject == null)
            {
                Debug.LogWarning($"Could not get Bullet from pool.");
                return;
            }
            
            if (bulletObject.TryGetComponent(out BulletImplementation bullet))
            {
                bullet.Fire(origin, direction, WeaponData.BulletData);
            }
            else
            {
                Debug.LogWarning($"Could not get component {nameof(BulletImplementation)} from bullet.", bullet);
            }
        }
    }
}