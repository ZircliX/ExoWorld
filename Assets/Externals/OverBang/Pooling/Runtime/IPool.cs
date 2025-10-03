using OverBang.Pooling.Resource;
using UnityEngine;

namespace OverBang.Pooling
{
    public interface IPool
    {
        PoolResource PoolResource { get; }
        
        void Load();
        void Dispose();

        Object Spawn();
        void Despawn(Object instance);
    }
}