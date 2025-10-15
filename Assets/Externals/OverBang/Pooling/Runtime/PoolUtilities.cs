using OverBang.Pooling.Resource;
using UnityEngine;

namespace OverBang.Pooling
{
    public static class PoolUtilities
    {
        public static T Spawn<T>(this PoolResource resource) where T : Object 
            => PoolManager.Instance.Spawn<T>(resource);
        
        public static void Despawn<T>(this T instance) where T : Object
            => PoolManager.Instance.Despawn(instance);
    }
}