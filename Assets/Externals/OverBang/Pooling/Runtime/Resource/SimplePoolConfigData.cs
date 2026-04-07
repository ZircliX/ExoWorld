using UnityEngine;

namespace OverBang.Pooling.Resource
{
    [CreateAssetMenu(fileName = "New SimplePoolConfig", menuName = "OverBang/Pooling/SimplePoolConfigData")]
    public class SimplePoolConfigData : ScriptableObject
    {
        [field: SerializeField] public SimplePoolConfig[] Config { get; private set; }
    }
}