using OverBang.Pooling;
using OverBang.Pooling.Resource;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
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
            GameObject instance = PoolManager.Instance.Spawn<GameObject>(resource);
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            
            return instance.GetComponent<NetworkObject>();
        }

        public void Destroy(NetworkObject networkObject)
        {
            PoolManager.Instance.Despawn(networkObject.gameObject);
        }
    }
}