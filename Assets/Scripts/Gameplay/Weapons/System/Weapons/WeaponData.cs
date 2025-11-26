using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [CreateAssetMenu(menuName = "OverBang/Weapons/WeaponData", fileName = "Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [field: SerializeField] public BulletData BulletData { get; protected set; }
        
        [field: SerializeField] public string WeaponName { get; protected set; }
        [field: SerializeField] public string WeaponDescription { get; protected set; }
        [field: SerializeField] public Sprite WeaponSprite { get; protected set; }
        [field: SerializeField] public GameObject WeaponPrefab { get; protected set; }
        
        [field: SerializeField] public WeaponFireBehaviour FireBehaviour { get; protected set; }
        [field: SerializeField] public float ShootingRate { get; protected set; }
        [field: SerializeField] public int BulletPerShot { get; protected set; }
        [field: SerializeField] public Vector2 BulletDispersion { get; protected set; }
        
        [field: SerializeField] public WeaponReloadBehaviour ReloadBehaviour { get; protected set; }
        [field: SerializeField] public int MagCapacity { get; protected set; }
        [field: SerializeField] public float ReloadTime { get; protected set; }
        
    }
}