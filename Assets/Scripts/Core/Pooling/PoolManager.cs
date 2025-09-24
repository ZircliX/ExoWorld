using System.Collections.Generic;
using OverBang.GameName.Core.GameAssets;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace OverBang.GameName.Core.Pooling
{
    public sealed class PoolManager
    {
        private Dictionary<string, Pool> pools;
        private List<PoolConfig> poolConfigs;
        private Transform parent;

        public static PoolManager Create(GameDatabase gameDatabase)
        {
            PoolManager poolManager = new PoolManager();
            return poolManager;
        }

        private PoolManager()
        {
            parent = new GameObject("Pools").transform;
            Object.DontDestroyOnLoad(parent);
        }

        public async Awaitable Initialize(GameDatabase gameDatabase)
        {
            poolConfigs = new List<PoolConfig>(8);
            pools = new Dictionary<string, Pool>(8);
            
            AsyncOperationHandle handle = gameDatabase.LoadMultiple("PoolConfig", ctx =>
            {
                if (ctx is PoolConfig poolConfig)
                {
                    Debug.Log($"Got a pool config : {poolConfig.PoolName}");
                    InitializePool(poolConfig);
                }
            });
            
            await handle.Task;

            WarmupPools();
        }

        private void InitializePool(PoolConfig poolConfig)
        {
            Pool newPool = new Pool(parent, poolConfig);
            pools.Add(poolConfig.PoolName, newPool);
            poolConfigs.Add(poolConfig);
        }

        public void WarmupPools()
        {
            foreach ((string key, Pool value) in pools)
            {
                value.WarmUp();
            }
        }

        public T Spawn<T>(string poolName) where T : Component, IPoolable
        {
            Pool pool = GetPool(poolName);
            if (pool == null)
            {
                Debug.LogError($"Pool with name {poolName} does not exist.");
                return null;
            }
            
            T obj = pool.GetObject<T>();
            if (obj == null)
            {
                Debug.LogError($"Failed to spawn object from pool {poolName}.");
                return null;
            }
            
            obj.OnSpawn();

            return obj;
        }
        
        public void Despawn<T>(T obj) where T : Component, IPoolable
        {
            Pool pool = GetPool(obj.PoolName);
            if (pool == null)
            {
                Debug.LogError($"Pool with name {obj.PoolName} does not exist.");
                return;
            }
            
            obj.OnDespawn();
            pool.ReturnObject(obj.gameObject);
        }
        
        public Pool GetPool(string poolName)
        {
            if (pools.TryGetValue(poolName, out Pool pool))
            {
                return pool;
            }
            Debug.LogWarning($"Pool with name {poolName} not found.");
            return null;
        }
        
        public void ClearAllPools()
        {
            foreach (Pool pool in pools.Values)
            {
                pool.Clear();
            }
            pools.Clear();
        }
    }
}