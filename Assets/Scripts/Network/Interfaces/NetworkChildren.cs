using Health.Network;
using UnityEngine;

namespace Network.Interfaces
{
    public abstract class NetworkChildren : MonoBehaviour
    {
        public abstract void OnNetworkSpawn(PlayerNetworkController playerNetwork);
        public abstract void OnNetworkDespawn();
        public abstract void OnNetworkUpdate();
    }
}