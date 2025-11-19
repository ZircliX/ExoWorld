using System;
using System.Collections.Generic;
using OverBang.Pooling;
using OverBang.Pooling.Dependencies;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.Pool;

namespace OverBang.GameName.Core
{
    public static class PoolUtils
    {
        public static async Awaitable SetupPooling(Action<List<IPoolDependencyProvider>> OnCollectSceneProviders)
        {
            using (ListPool<IPoolDependencyProvider>.Get(out List<IPoolDependencyProvider> providers))
            {
                CharacterData[] characterData = Resources.LoadAll<CharacterData>("Characters");
                providers.AddRange(characterData);
                
                OnCollectSceneProviders?.Invoke(providers);

                PoolDependenciesCollector collector = new PoolDependenciesCollector();
                foreach (IPoolConfig config in collector.Collect(providers))
                    PoolManager.Instance.RegisterPool(config);
            }
            
            await Awaitable.MainThreadAsync();
        }
    }
}