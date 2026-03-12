using OverBang.ExoWorld.Core.Metrics;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Utils
{
    public static class NetworkManagerBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            NetworkManager prefab = GameMetrics.Global.NetworkManager;
            if (prefab == null)
            {
                Debug.LogError("[NetworkManagerBootstrapper] NetworkManager prefab not found in Resources folder.");
                return;
            }

            NetworkManager instance = Object.Instantiate(prefab);
            Object.DontDestroyOnLoad(instance);
        }
    }
}