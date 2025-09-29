using System.Collections.Generic;
using OverBang.Pooling.Resource;
using UnityEngine;

namespace OverBang.Pooling.PoolStrategies
{
    public class ComponentPoolStrategy : PoolStrategy<Component>
    {
        protected override void PreparePooledInstance(Component asset)
        {
            asset.gameObject.SetActive(false);
        }
        
        protected override void CollectListeners(Component instance, List<IPoolInstanceListener> listeners)
        {
            instance.GetComponentsInChildren(true, listeners);
        }

        protected override void OnPostSpawn(Component instance)
        {
            instance.gameObject.SetActive(true);
        }

        protected override void OnPostDestroy(Component instance)
        {
            instance.gameObject.SetActive(false);
        }
    }
}