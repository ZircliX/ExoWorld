using Helteix.Singletons.MonoSingletons;
using OverBang.GameName.Core;
using OverBang.GameName.Gameplay;
using OverBang.Pooling.Resource;
using Unity.Netcode;

namespace OverBang.Pooling
{
    public class PoolAssetNetworkHandler : MonoSingleton<PoolAssetNetworkHandler>
    {
        private void OnEnable()
        {
            SceneLoader.OnPreSceneLoad += Desactivate;
            SceneLoader.OnPostSceneLoad += Activate;
        }

        private void OnDisable()
        {
            SceneLoader.OnPreSceneLoad -= Desactivate;
            SceneLoader.OnPostSceneLoad -= Activate;
        }

        private void Activate()
        {
            PoolManager.Instance.OnPoolAssetRegistered += OnPoolAssetRegistered;
            PoolManager.Instance.OnPoolAssetUnregistered += OnPoolAssetUnregistered;
        }

        private void Desactivate()
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