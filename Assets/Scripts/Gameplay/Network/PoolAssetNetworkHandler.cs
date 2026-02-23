using Helteix.Singletons.SceneServices;
using OverBang.ExoWorld.Gameplay.Level;
using OverBang.Pooling;
using OverBang.Pooling.Resource;
using Unity.Netcode;

namespace OverBang.ExoWorld.Gameplay.Network
{
    public class PoolAssetNetworkHandler : SceneService<PoolAssetNetworkHandler>
    {
        private void OnEnable()
        {
            PoolManager.Instance.OnPoolAssetRegistered += OnPoolAssetRegistered;
            PoolManager.Instance.OnPoolAssetUnregistered += OnPoolAssetUnregistered;
        }

        private void OnDisable()
        {
            PoolManager.Instance.OnPoolAssetRegistered -= OnPoolAssetRegistered;
            PoolManager.Instance.OnPoolAssetUnregistered -= OnPoolAssetUnregistered;
        }

        private void OnPoolAssetRegistered(PoolResource resource)
        {
            switch (resource.Asset)
            {
                case PrefabPoolAsset prefabPoolAsset:
                    if (!prefabPoolAsset.Prefab.TryGetComponent(out NetworkObject networkObject))
                        return;
                    
                    //Debug.LogError($"Registering prefab {prefabPoolAsset.Prefab.name} as network prefab");
                    PoolingNetworkPrefabHandler networkPrefabHandler = new PoolingNetworkPrefabHandler(resource);
                    NetworkManager.Singleton.PrefabHandler.AddHandler(networkObject, networkPrefabHandler);
                    break;
            }
        }
        private void OnPoolAssetUnregistered(PoolResource resource)
        {
            switch (resource.Asset)
            {
                case PrefabPoolAsset prefabPoolAsset:
                    
                    if (!prefabPoolAsset.Prefab.TryGetComponent(out NetworkObject networkObject))
                        return;
                    NetworkManager.Singleton.PrefabHandler.RemoveHandler(networkObject);
                    break;
            }
        }
    }
}