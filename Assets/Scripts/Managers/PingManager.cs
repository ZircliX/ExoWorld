using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Managers
{
    public class PingManager : NetworkBehaviour
    {
        [SerializeField] private float pingActualisationTime = 2f;
        
        [field: SerializeField] public Dictionary<ulong, float> Ping { get; private set; }
        
        private float currentTime;

        private void Awake()
        {
            Ping = new Dictionary<ulong, float>(4);
        }

        private void Update()
        {
            if (!IsClient || !IsOwner) return;
            
            if (currentTime >= pingActualisationTime)
            {
                currentTime = 0;
                SendPingServerRpc(Time.realtimeSinceStartup);
            }
            else
            {
                currentTime += Time.deltaTime;
            }
        }

        [Rpc(SendTo.Server)]
        private void SendPingServerRpc(float sentTime, RpcParams rpcParams = default)
        {
            SendPingClientRpc(Time.realtimeSinceStartup - sentTime, rpcParams.Receive.SenderClientId);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SendPingClientRpc(float sentTime, ulong clientId)
        {
            float rtt = sentTime;

            Ping[clientId] = rtt;
        }

        public float GetPlayerPing(ulong playerID)
        {
            float ping = -1;
            Ping.TryGetValue(playerID, out ping);
            return ping;
        }
    }
}