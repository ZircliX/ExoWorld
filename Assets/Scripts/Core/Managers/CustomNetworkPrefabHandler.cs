using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName
{
    public class CustomNetworkPrefabHandler : INetworkPrefabInstanceHandler
    {
        public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            return null;
        }

        public void Destroy(NetworkObject networkObject)
        {
            return ;
        }
    }
}