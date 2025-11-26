using OverBang.Pooling.Resource;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [CreateAssetMenu(fileName = "BulletData", menuName = "OverBang/Weapons/BulletData")]
    public class BulletData : ScriptableObject
    {
        [System.Serializable]
        public struct BulletDamageInfo
        {
            public float bodyDamage;
            public float headDamage;
        }
        
        [field: SerializeField] public PoolResource BulletPoolResource { get; protected set; }
        
        [field: SerializeField] public float BulletSpeed { get; protected set; }
        [field: SerializeField] public BulletDamageInfo BulletDamage { get; protected set; }
    }
}