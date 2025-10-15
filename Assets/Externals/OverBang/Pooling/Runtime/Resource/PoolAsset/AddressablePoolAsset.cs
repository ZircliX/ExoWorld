using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace OverBang.Pooling.Resource
{
    [System.Serializable]
    public struct AddressablePoolAsset : IPoolAsset
    {
        [field: SerializeField]
        public AssetReferenceGameObject AssetReference { get; private set; }
        
        public void Load(System.Action<Object> onLoad)
        {
            AssetReference.LoadAssetAsync<Object>().Completed += handle =>
            {
                onLoad?.Invoke(handle.Result);
            };
        }

        public void Unload()
        {
            AssetReference.ReleaseAsset();
        }
    }
}