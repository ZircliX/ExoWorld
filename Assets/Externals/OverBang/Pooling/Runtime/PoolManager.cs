using System;
using System.Collections.Generic;
using OverBang.Pooling.Resource;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OverBang.Pooling
{
    public class PoolManager : Helteix.Singletons.MonoSingletons.MonoSingleton<PoolManager>
    {
        public event Action<PoolResource> OnPoolAssetRegistered; 
        public event Action<PoolResource> OnPoolAssetUnregistered;
        public event Action OnPoolsLoaded;
        
        private Dictionary<IPoolConfig, IPool> pools;
        private Dictionary<IPool, bool> poolsLoadStatus;
        private List<IPool> poolList;
        
        private Transform poolParent;
        private Dictionary<Object, IPool> instanceToPool;

        protected override void OnAwake()
        {
            poolParent = new GameObject($"Pools_Root").transform;

            pools = new Dictionary<IPoolConfig, IPool>();
            poolsLoadStatus = new Dictionary<IPool, bool>();
            poolList = new List<IPool>();
            
            instanceToPool = new Dictionary<Object, IPool>();
        }

        public void RegisterPools(params IPoolConfig[] poolConfigs)
        {
            foreach (IPoolConfig poolConfig in poolConfigs)
                RegisterPool(poolConfig);
        }

        public void RegisterPool(IPoolConfig poolConfig)
        {
            if (pools.ContainsKey(poolConfig))
                return;

            IPool pool = null;

            PoolResource poolResource = poolConfig.PoolResource;
            switch (poolResource.Asset)
            {
                case PrefabPoolAsset prefabAsset:
                    pool = new Pool<GameObject>(poolParent, poolConfig);
                    OnPoolAssetRegistered?.Invoke(poolResource);
                    break;

                case ResourcePoolAsset resourceAsset:
                    pool = new Pool<Object>(poolParent, poolConfig);
                    OnPoolAssetRegistered?.Invoke(poolResource);
                    break;

                default:
                    Debug.LogError($"Unsupported pool asset type {poolResource.Asset.GetType()}");
                    break;
            }

            if (pool != null)
            {
                pools[poolConfig] = pool;
                poolsLoadStatus[pool] = false;
                pool.OnFillComplete += CheckForCompleteFill;
                pool.Load();
            }
        }

        private void CheckForCompleteFill(IPool pool)
        {
            poolsLoadStatus[pool] = true;
            pool.OnFillComplete -= CheckForCompleteFill;
            
            foreach (bool status in poolsLoadStatus.Values)
            {
                if (!status) return;
                
                OnPoolsLoaded?.Invoke();
            }
        }

        public void UnregisterPools(params PoolConfigAsset[] poolConfigs)
        {
            foreach (PoolConfigAsset config in poolConfigs)
            {
                if(UnregisterPool(config))
                    OnPoolAssetUnregistered?.Invoke(config.PoolResource);
            }
        }

        public bool UnregisterPool(PoolConfigAsset config)
        {
            if (pools.Remove(config, out IPool pool))
            {
                pool.Dispose();
                return true;
            }
            return false;
        }

        public void ClearPools()
        {
            foreach (IPool pool in pools.Values)
            {
                pool.Dispose();
            }
            
            pools.Clear();
            poolList.Clear();
        }

        public T Spawn<T>(PoolResource resource) where T : Object
        {
            foreach (IPool pool in pools.Values)
            {
                if (pool.PoolResource.Asset == resource.Asset)
                {
                    if (TryGetInstance(pool, out T instance))
                    {
                        instanceToPool.TryAdd(instance, pool);
                        return instance;
                    }
                }
            }
            
            Debug.LogError($"No pool registered for resource {resource.name}");
            return null;
        }
        
        public async Awaitable Despawn<T>(T instance, float time) where T : Object
        {
            await Awaitable.WaitForSecondsAsync(time);
            Despawn(instance);
        }

        public void Despawn<T>(T instance) where T : Object
        {
            if (instanceToPool.TryGetValue(instance, out IPool pool))
            {
                pool.Despawn(instance);
                instanceToPool.Remove(instance);
            }
            else
            {
                if (instance == null)
                    Debug.LogError("Tried to despawn a null instance.");
                else
                    Debug.LogError($"Tried to despawn {instance.GetType()} in pool of {typeof(T)}");
            }
        }

        private bool TryGetInstance<T>(IPool pool, out T instance) where T : Object
        {
            Object pooledObject = pool.Spawn();
            if (pooledObject is T typed)
            {
                instance = typed;
                return true;
            }

            string type = pooledObject != null ? pooledObject.GetType().ToString() : "Null";
            Debug.LogError($"Pool for {pool.PoolResource.name} spawned {type}, but requested {typeof(T)}");
            pool.Despawn(pooledObject);

            instance = null;
            return false;
        }
    }
}