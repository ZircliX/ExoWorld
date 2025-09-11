using System;
using LTX.Singletons;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OverBang.GameName.Core.Scene
{
    public class SceneManager : NetworkBehaviour
    {
        
        public static event Action OnInstanceCreated;
        
        public static SceneManager Instance { get; private set; }
        public static bool HasInstance => Instance != null;

        public override void OnNetworkSpawn()
        {
            // Enforce singleton on all instances (server & clients)
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Multiple SceneManager instances detected. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            OnInstanceCreated?.Invoke();
        }

        public void ChangeScene(string sceneName)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                Debug.Log("Scene Changed !");

                NetworkManager.Singleton.SceneManager.LoadScene(sceneName,LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError("Only Host Can Change Scene !");
            }
        }
        
    }
}