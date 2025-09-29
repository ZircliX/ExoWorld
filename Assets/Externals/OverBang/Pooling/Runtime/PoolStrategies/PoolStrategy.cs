using System.Collections.Generic;
using OverBang.Pooling.Resource;
using UnityEngine;

namespace OverBang.Pooling.PoolStrategies
{
    public abstract class PoolStrategy<T>  : IPoolStrategy where T : Object
    {
        protected virtual void ProcessLoadedAsset(T asset) { }
        protected virtual void CollectListeners(T instance, List<IPoolInstanceListener> listeners) { }
        protected virtual void PreparePooledInstance(T asset) { }

        protected virtual void OnPreSpawn(T instance) { }
        protected virtual void OnPostSpawn(T instance) { }

        protected virtual void OnPreDestroy(T instance) { }
        protected virtual void OnPostDestroy(T instance) { }

        void IPoolStrategy.ProcessLoadedAsset(Object asset)
        {
            if(asset is T t)
                ProcessLoadedAsset(t);
        }

        void IPoolStrategy.CollectListeners(Object instance, List<IPoolInstanceListener> listeners)
        {
            if(instance is T t)
                CollectListeners(t, listeners);
        }

        void IPoolStrategy.PreparePooledInstance(Object instance)
        {
            if(instance is T t)
                PreparePooledInstance(t);
        }

        void IPoolStrategy.OnPreSpawn(Object instance)
        {
            if(instance is T t)
                OnPreSpawn(t);
        }

        void IPoolStrategy.OnPostSpawn(Object instance)
        {
            if(instance is T t)
                OnPostSpawn(t);
        }

        void IPoolStrategy.OnPreDestroy(Object instance)
        {
            if(instance is T t)
                OnPreDestroy(t);
        }

        void IPoolStrategy.OnPostDestroy(Object instance)
        {
            if(instance is T t)
                OnPostDestroy(t);
        }
    }
}