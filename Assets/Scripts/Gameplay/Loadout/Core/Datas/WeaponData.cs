using Ami.BroAudio;
using OverBang.ExoWorld.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
{
    [CreateAssetMenu(menuName = "OverBang/Weapons/WeaponData", fileName = "Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        // BULLET DATA
        [field: SerializeField] public BulletData BulletData { get; protected set; }
        
        // GENERAL DATA
        [field: SerializeField] public string WeaponName { get; protected set; }
        [field: SerializeField] public string WeaponType { get; protected set; }
        [field: SerializeField] public string WeaponDescription { get; protected set; }
        [field: SerializeField] public Sprite WeaponSprite { get; protected set; }
        [field: SerializeField] public Weapon Prefab { get; protected set; }
        
        // Audio DATA
        [field: SerializeField] public SoundID FireSound { get; protected set; }
        
        // FIRE BEHAVIOUR
        [field: SerializeField] public WeaponFireBehaviour FireBehaviour { get; protected set; }
        [field: SerializeField] public float FireCooldown { get; protected set; }
        [field: SerializeField] public int BulletsPerShot { get; protected set; }
        
        // RECOIL DATA
        [field: SerializeField] public int RecoilPatternShots { get; protected set; }
        [field: SerializeField] public int MaxRecoilShots { get; protected set; }
        [field: SerializeField] public Vector2 BulletDispersionX { get; protected set; }
        [field: SerializeField] public AnimationCurve RecoilPatternX { get; protected set; }
        [field: SerializeField] public Vector2 BulletDispersionY { get; protected set; }
        [field: SerializeField] public AnimationCurve RecoilPatternY { get; protected set; }
        [field: SerializeField] public float CurveMultiplier { get; protected set; }
        [field: SerializeField] public float RandomnessMultiplier { get; protected set; }
        [field: SerializeField] public float ResetLerpSpeed { get; protected set; }
        
        // RELOAD DATA
        [field: SerializeField] public WeaponReloadBehaviour ReloadBehaviour { get; protected set; }
        [field: SerializeField] public int MagCapacity { get; protected set; }

        public int UpgradeMagCap => Mathf.CeilToInt(MagCapacity *
                                    (UpgradeManager.Instance.GetRuntimeUpgrade(UpgradeType.MaxMagCap)/
                                     100));
        [field: SerializeField] public float ReloadTime { get; protected set; }
    }
}