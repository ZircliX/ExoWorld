using System;
using UnityEngine;

namespace OverBang.Pooling.Resource
{
    [Serializable]
    public struct SimplePoolConfig : IPoolConfig
    {
        [field: SerializeField]
        public int PoolSize { get; private set; }
        [field: SerializeField]
        public PoolResource PoolResource { get; private set; }
        
        public bool IsValid => PoolResource != null && PoolSize > 0;

        public static implicit operator bool(SimplePoolConfig config) => config.IsValid;
    }
}