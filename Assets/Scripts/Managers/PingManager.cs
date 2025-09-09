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
            if (!IsClient) return;

            currentTime += Time.deltaTime;
            if (currentTime >= pingActualisationTime)
            {
                currentTime = 0;
                SendPingServerRpc(Time.realtimeSinceStartup);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SendPingServerRpc(float sentTime, ServerRpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;
            float rtt = Time.realtimeSinceStartup - sentTime;

            // Update server's dictionary
            Ping[clientId] = rtt * 1000; // ms

            // Broadcast to all clients (including host)
            UpdatePingClientRpc(clientId, rtt * 1000);
        }

        [ClientRpc]
        private void UpdatePingClientRpc(ulong clientId, float rtt)
        {
            Ping[clientId] = rtt;
        }

        public float GetPlayerPing(ulong playerID)
        {
            return Ping.GetValueOrDefault(playerID, -1);
        }
    }
}