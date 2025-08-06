using UnityEngine;

namespace OverBang.GameName.Network
{
    public abstract class NetworkChildren : MonoBehaviour
    {
        public virtual void OnNetworkSpawn(PlayerNetworkController playerNetwork)
        {
            
        }

        public virtual void OnNetworkDespawn()
        {
            
        }

        public virtual void OnUpdate()
        {
            
        }
    }
}