using UnityEngine;

namespace OverBang.Pooling.Resource
{
    [CreateAssetMenu(fileName = "New PoolConfigAsset", menuName = "OverBang/Pooling/Config")]
    public class PoolConfigAsset : ScriptableObject, IPoolConfig
    {
        [field: SerializeField, Min(0)] public int PoolSize { get; private set; }
        [field: SerializeField] public PoolResource PoolResource { get; private set; }
    }
}