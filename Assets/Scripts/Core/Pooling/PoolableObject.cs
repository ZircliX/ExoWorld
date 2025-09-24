using UnityEngine;

namespace OverBang.GameName.Core.Pooling
{
    public abstract class PoolableObject : MonoBehaviour, IPoolable
    {
        public string PoolName { get; set; }
        public bool IsInUse { get; set; }

        public abstract void OnSpawn();

        public abstract void OnDespawn();
    }
}