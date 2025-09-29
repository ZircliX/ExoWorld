using UnityEngine;
using Object = UnityEngine.Object;

namespace OverBang.Pooling.Resource
{
    [System.Serializable]
    public struct ResourcePoolAsset : IPoolAsset
    {
        [field: SerializeField]
        public string Path { get; private set; }
        
        public void Load(System.Action<Object> onLoad)
        {
            ResourceRequest op = Resources.LoadAsync(Path);
            op.completed += operation =>
            {
                onLoad?.Invoke(((ResourceRequest)operation).asset);
            };
        }

        public void Unload()
        {
            
        }
    }
}