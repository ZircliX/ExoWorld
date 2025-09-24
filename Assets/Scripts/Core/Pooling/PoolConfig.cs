using UnityEngine;

namespace OverBang.GameName.Core.Pooling
{
    [CreateAssetMenu(fileName = "New PoolConfig", menuName = "OverBang/Pooling/PoolConfig")]
    public class PoolConfig : ScriptableObject
    {
        [field: SerializeField] public string PoolName { get; private set; }
        [field: SerializeField] public GameObject PrefabRef { get; private set; }
        [field: SerializeField] public int InitialSize { get; private set; }
        [field: SerializeField] public bool Expendable { get; private set; }
    }
}