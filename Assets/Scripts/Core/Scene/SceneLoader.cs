using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace OverBang.GameName.Core.Scenes
{
    public static class SceneLoader
    {
        public static string GetCurrentSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }

        public static SceneEventProgressStatus NetworkLoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            return NetworkManager.Singleton.SceneManager.LoadScene(sceneName, mode);
        }
        
        public static async Awaitable<Scene> LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool setActive = true)
        {
            await UnitySceneManager.LoadSceneAsync(sceneName, new LoadSceneParameters() { loadSceneMode = mode });

            Scene scene = UnitySceneManager.GetSceneByName(sceneName);
            if (setActive)
                UnitySceneManager.SetActiveScene(scene);
            
            return scene;
        }
        
        public static async Awaitable UnloadSceneAsync(string sceneName)
        {
            if (!UnitySceneManager.GetSceneByName(sceneName).isLoaded)
            {
                Debug.LogWarning($"La scène {sceneName} n'est pas chargée.");
                return;
            }
            
            await UnitySceneManager.UnloadSceneAsync(sceneName);
        }
    }
}