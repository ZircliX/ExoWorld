using UnityEngine;

namespace OverBang.Pooling.PoolStrategies
{
    public static class PoolStrategyUtilities
    {
        public static IPoolStrategy GetStrategyFor(Object asset) =>
            asset switch
            {
                GameObject => new PrefabPoolStrategy(),
                Component => new ComponentPoolStrategy(),
                ScriptableObject => new ScriptablePoolStrategy(),
                _ => null,
            };
    }
}