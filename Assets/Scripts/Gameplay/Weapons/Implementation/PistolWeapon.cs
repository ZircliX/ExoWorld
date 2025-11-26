using OverBang.Pooling;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class PistolWeapon : Weapon
    {
        [SerializeField] private Transform shootPoint;

        public override void Fire()
        {
            GameObject bullet = WeaponData.BulletData.BulletPoolResource.Spawn<GameObject>();
            if (bullet == null)
            {
                Debug.LogWarning($"Could not get Bullet {bullet.GetType().Name} from pool.");
                return;
            }
            
            if (bullet.TryGetComponent(out PistolBullet debugBullet))
            {
                Vector3 dir = GetBulletDirection(shootPoint, WeaponData.BulletDispersion);
                debugBullet.Fire(shootPoint, dir, WeaponData.BulletData.BulletSpeed);
            }
        }
    }
}