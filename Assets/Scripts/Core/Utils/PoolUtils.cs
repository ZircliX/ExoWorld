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
        [Flags]
        public enum PoolType
        {
            Characters = 1 << 0,
            Dependencies = 1 << 1,
            Enemies = 1 << 2,
            
            All = Characters | Enemies
        }
        
        public static IEnumerable<IPoolDependencyProvider> CollectProviders(PoolType poolType = PoolType.All)
        {
            if ((poolType & PoolType.Characters) != 0)
            {
                CharacterData[] characterData = Resources.LoadAll<CharacterData>("Characters");
                foreach (CharacterData character in characterData)
                    yield return character;
            }
        
            if ((poolType & PoolType.Enemies) != 0)
            {
                EnemyData[] enemyData = Resources.LoadAll<EnemyData>("Enemies");
                foreach (EnemyData enemy in enemyData)
                    yield return enemy;
            }
        }
        
        public static async Awaitable SetupPooling(PoolType poolType = PoolType.All)
        {
            using (ListPool<IPoolDependencyProvider>.Get(out List<IPoolDependencyProvider> providers))
            {
                // Collect only the desired providers
                providers.AddRange(CollectProviders(poolType));
            
                PoolDependenciesCollector collector = new PoolDependenciesCollector();
                foreach (IPoolConfig config in collector.Collect(providers))
                    PoolManager.Instance.RegisterPool(config);
            }
        
            await Awaitable.MainThreadAsync();
        }
    }
}