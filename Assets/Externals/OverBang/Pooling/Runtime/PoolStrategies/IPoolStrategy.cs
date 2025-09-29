using System.Collections.Generic;
using OverBang.Pooling.Resource;
using UnityEngine;

namespace OverBang.Pooling.PoolStrategies
{
    public interface IPoolStrategy
    {
        public void ProcessLoadedAsset(Object asset);

        public void CollectListeners(Object instance, List<IPoolInstanceListener> listeners);
        public void PreparePooledInstance(Object instance);
        
        public void OnPreSpawn(Object instance);
        
        public void OnPostSpawn(Object instance);
        
        public void OnPreDestroy(Object instance);
        
        public void OnPostDestroy(Object instance);
    }
}