using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace OverBang.GameName.Core.Scene
{
    public static class SceneLoader
    {
        public static Task LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            AsyncOperation op = UnitySceneManager.LoadSceneAsync(sceneName, mode);

            if (op != null) 
                op.completed += _ => tcs.SetResult(true);

            return tcs.Task;
        }
        
        public static IEnumerator PreloadSceneServerOnly(string sceneName)
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
    }
}