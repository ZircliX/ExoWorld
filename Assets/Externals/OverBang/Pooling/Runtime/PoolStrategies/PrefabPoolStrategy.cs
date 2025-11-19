using System.Collections.Generic;
using OverBang.Pooling.Resource;
using UnityEngine;

namespace OverBang.Pooling.PoolStrategies
{
    public class PrefabPoolStrategy : PoolStrategy<GameObject>
    {
        protected override void PreparePooledInstance(GameObject asset)
        {
            asset.SetActive(false);
        }

        protected override void CollectListeners(GameObject instance, List<IPoolInstanceListener> listeners)
        {
            instance.GetComponentsInChildren(true, listeners);
        }

        protected override void OnPostSpawn(GameObject instance)
        {
            if (instance == null)
            {
                Debug.LogWarning($"Object instance : {instance.name} on prefab pool strategy was null.");
            }
            else
            {
                instance.SetActive(true);
            }
        }

        protected override void OnPostDestroy(GameObject instance)
        {
            if (instance == null)
            {
                Debug.LogWarning($"Object instance : {instance.name} on prefab pool strategy was null.");
            }
            else
            {
                instance.SetActive(false);
            }
        }
    }
}