using UnityEngine;

namespace OverBang.GameName.Gameplay.Combat
{
    public abstract class WeaponData : ScriptableObject
    {
        [field: SerializeField] public BulletDamageInfo DamageInfo { get; protected set; }
        
        [field: SerializeField] public float AmmoPerMag { get; protected set; }
        [field: SerializeField] public float AmmoPerSec { get; protected set; }
        [field: SerializeField] public float ReloadTime { get; protected set; }
    }
}