using System.Collections.Generic;
using OverBang.Pooling.Resource;
using UnityEngine.Pool;

namespace OverBang.Pooling.Dependencies
{
    public struct PoolDependenciesCollector
    {
        private struct Config : IPoolConfig
        {
            public int PoolSize { get; set; }
            public PoolResource PoolResource { get; set; }
        }
        
        public IEnumerable<IPoolConfig> Collect(List<IPoolDependencyProvider> providers)
        {
            using (DictionaryPool<PoolResource, int>.Get(out Dictionary<PoolResource, int> finalConfigDic))
            {
                foreach (IPoolDependencyProvider provider in providers)
                    ExtractConfigsFromProvider(provider, finalConfigDic);

                foreach ((PoolResource poolResource, int quantity) in finalConfigDic)
                    yield return new Config { PoolResource = poolResource, PoolSize = quantity };
            }
        }

        private static void ExtractConfigsFromProvider(
            IPoolDependencyProvider provider,
            Dictionary<PoolResource, int> finalConfigDic)
        {
            using (ListPool<IPoolConfig>.Get(out List<IPoolConfig> configs))
            {
                provider.FillDependencies(configs);

                foreach (IPoolConfig config in configs)
                {
                    // Aggregate quantities
                    if (finalConfigDic.TryGetValue(config.PoolResource, out int current))
                        finalConfigDic[config.PoolResource] = current + config.PoolSize;
                    else
                        finalConfigDic.Add(config.PoolResource, config.PoolSize);

                    // Recurse if nested provider
                    if (config.PoolResource is IPoolDependencyProvider innerProvider)
                        ExtractConfigsFromProvider(innerProvider, finalConfigDic);
                }
            }
        }
    }
}
