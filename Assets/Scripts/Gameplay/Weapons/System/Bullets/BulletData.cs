using OverBang.Pooling.Resource;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [CreateAssetMenu(fileName = "BulletData", menuName = "OverBang/Weapons/BulletData")]
    public class BulletData : ScriptableObject
    {
        [field: SerializeField] public NetworkObject BulletPrefab { get; protected set; }
        
        [field: SerializeField] public float BulletSpeed { get; protected set; }
        [field: SerializeField] public int Penetration { get; protected set; }
        [field: SerializeField] public DamageInfo Damage { get; protected set; }
    }
}
