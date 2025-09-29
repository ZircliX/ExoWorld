using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OverBang.Pooling.Resource
{
    [System.Serializable]
    public struct PrefabPoolAsset : IPoolAsset
    {
        [field: SerializeField]
        public GameObject Prefab { get; private set; }
        
        public void Load(Action<Object> onLoad)
        {
            onLoad?.Invoke(Prefab);
        }

        public void Unload()
        {
            
        }
    }
}