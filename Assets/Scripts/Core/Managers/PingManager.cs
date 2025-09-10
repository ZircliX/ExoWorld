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
                MeasureAndSendPing();
            }
        }

        private void MeasureAndSendPing()
        {
            float startTime = Time.realtimeSinceStartup;
            SendPingServerRpc(startTime);
        }

        // The client sends its local timestamp
        [ServerRpc(RequireOwnership = false)]
        private void SendPingServerRpc(float startTime, ServerRpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;

            // Immediately echo back to the client that sent the ping
            EchoPingClientRpc(startTime, new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
            });
        }

        // Client receives echo and calculates RTT locally
        [ClientRpc]
        private void EchoPingClientRpc(float startTime, ClientRpcParams rpcParams = default)
        {
            float rtt = (Time.realtimeSinceStartup - startTime) * 1000f; // ms
            ulong clientId = NetworkManager.Singleton.LocalClientId;

            // Update local dictionary
            Ping[clientId] = rtt;

            // Send measured RTT to server for storage/broadcast if needed
            SendMeasuredPingToServerRpc(rtt);
        }

        // Optional: server stores RTT to broadcast to others
        [ServerRpc(RequireOwnership = false)]
        private void SendMeasuredPingToServerRpc(float rtt, ServerRpcParams rpcParams = default)
        {
            ulong clientId = rpcParams.Receive.SenderClientId;

            // Store RTT on server
            Ping[clientId] = rtt;

            // Broadcast to all clients
            UpdatePingClientRpc(clientId, rtt);
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