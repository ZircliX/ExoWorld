using System;
using System.Collections.Generic;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.Enemies;
using OverBang.Pooling;
using OverBang.Pooling.Dependencies;
using UnityEngine;
using UnityEngine.Pool;

namespace OverBang.ExoWorld.Core.Utils
{
    public static class PoolUtils
    {
        public static async Awaitable SetupPooling(Action<List<IPoolDependencyProvider>> OnCollectSceneProviders = null)
        {
            using (ListPool<IPoolDependencyProvider>.Get(out List<IPoolDependencyProvider> providers))
            {
                CharacterData[] characterData = Resources.LoadAll<CharacterData>("Characters");
                providers.AddRange(characterData);
                
                EnemyData[] enemyData = Resources.LoadAll<EnemyData>("Enemies");
                providers.AddRange(enemyData);
                
                OnCollectSceneProviders?.Invoke(providers);

                PoolDependenciesCollector collector = new PoolDependenciesCollector();
                foreach (IPoolConfig config in collector.Collect(providers))
                    PoolManager.Instance.RegisterPool(config);
            }
            
            await Awaitable.MainThreadAsync();
        }
    }
}