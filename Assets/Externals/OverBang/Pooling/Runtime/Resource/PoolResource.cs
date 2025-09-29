using UnityEngine;

namespace OverBang.Pooling.Resource
{
    [CreateAssetMenu(fileName = "New PoolResource", menuName = "OverBang/Pooling/Resources")]
    public class PoolResource : ScriptableObject
    {
        [field: SerializeField] public PoolEmptyBehavior PoolEmptyBehavior { get; private set; }
        [field: SerializeReference] public IPoolAsset Asset { get; private set; }
    }
}