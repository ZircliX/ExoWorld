using System;
using OverBang.Pooling.Resource;
using Object = UnityEngine.Object;

namespace OverBang.Pooling
{
    public interface IPool
    {
        PoolResource PoolResource { get; }
        event Action<IPool> OnFillComplete; 
        
        void Load();
        void Dispose();

        Object Spawn();
        void Despawn(Object instance);
    }
}