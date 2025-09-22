using System;
using System.Collections;
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
        
        public IEnumerator PreloadSceneServerOnly(string sceneName)
        {
            UnityEngine.SceneManagement.Scene targetScene =  UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
            if (targetScene.isLoaded)
            {
                Debug.Log($"[Server] La scène {sceneName} est déjà préchargée.");
                yield break;
            }

            Debug.Log($"[Server] Préchargement de la scène {sceneName}...");
            AsyncOperation asyncOp = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            while (!asyncOp.isDone)
            {
                yield return null;
            }

            Debug.Log($"[Server] Scène {sceneName} préchargée avec succès !");
        }
        
        
        public void ActivatePreloadedScene(string sceneName)
        {
            if (!IsServer)
                return;

            Debug.Log($"[Server] Activation de la scène {sceneName} pour tous les joueurs...");
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}