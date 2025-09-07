using System;
using System.Collections;
using OverBang.GameName.Metrics;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Network.Static
{
    public static class NetworkHelpers
    {
        public static IEnumerator CanRunNetworkOperation(this NetworkBehaviour behavior, Action onFinish)
        {
            float timer = 0f;
            while (timer < GameMetrics.Global.MaxDelay)
            {
                if (behavior.IsSpawned && 
                    behavior.NetworkManager && 
                    behavior.NetworkManager.IsListening)
                {
                    onFinish?.Invoke();
                    yield break;
                }
                
                timer += Time.deltaTime;
            }
            
            Debug.LogError($"{behavior.gameObject.name}TIMEOUT ERROR, CANNOT PERFORM NETWORK OPERATION !", behavior.gameObject);
        }
    }
}