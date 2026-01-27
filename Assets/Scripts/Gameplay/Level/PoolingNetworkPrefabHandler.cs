using OverBang.Pooling;
using OverBang.Pooling.Resource;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Level
{
    public class PoolingNetworkPrefabHandler : INetworkPrefabInstanceHandler
    {
        private readonly PoolResource resource;

        public PoolingNetworkPrefabHandler(PoolResource resource)
        {
            this.resource = resource;
        }

        public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            GameObject go = resource.Spawn<GameObject>();
            if (go != null && go.TryGetComponent(out NetworkObject networkObject))
            {
                //Debug.Log($"Spawning  {networkObject.name} network object from pool", networkObject);
                networkObject.ActiveSceneSynchronization = true;
                networkObject.transform.position = position;
                networkObject.transform.rotation = rotation;
                
                return networkObject;
            }

            return null;
        }

        public void Destroy(NetworkObject networkObject)
        {
            GameObject instance = networkObject.gameObject;
            //Debug.Log($"Return {instance.name} network object to pool", instance);
            PoolManager.Instance.Despawn(instance);
        }
    }
}