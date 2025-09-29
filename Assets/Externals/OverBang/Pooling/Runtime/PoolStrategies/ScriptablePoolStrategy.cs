using System.Collections.Generic;
using OverBang.Pooling.Resource;
using UnityEngine;

namespace OverBang.Pooling.PoolStrategies
{
    public class ScriptablePoolStrategy : PoolStrategy<ScriptableObject>
    {
        protected override void CollectListeners(ScriptableObject instance, List<IPoolInstanceListener> listeners)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (instance is IPoolInstanceListener listener)
            {
                listeners.Add(listener);
            }
        }
    }
}