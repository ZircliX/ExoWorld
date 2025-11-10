using System;
using System.Threading;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OverBang.GameName.Core
{
    public static class SceneLoader
    {
        public static Scene GetCurrentScene() => SceneManager.GetActiveScene();

        public static void SetActiveScene(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene.isLoaded) SceneManager.SetActiveScene(scene);
        }

        static async Awaitable AwaitSceneEvent(SceneEventType eventType, string sceneName, Func<SceneEventProgressStatus> op)
        {
            bool done = false;

            void Handler(SceneEvent e)
            {
                // Some events report name via e.Scene or e.SceneName; handle both safely.
                string name = string.IsNullOrEmpty(e.SceneName) ? e.Scene.name : e.SceneName;
                if (name == sceneName && e.SceneEventType == eventType) done = true;
            }

            NetworkSceneManager sm = NetworkManager.Singleton.SceneManager;
            sm.OnSceneEvent += Handler;
            try
            {
                op();
                await AwaitableUtils.AwaitableUntil(() => done, CancellationToken.None);
            }
            finally
            {
                sm.OnSceneEvent -= Handler;
            }
        }
        
        public static SceneEventProgressStatus NetworkLoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                Debug.LogWarning($"NetworkLoadScene ignored on client: {sceneName}");
                return SceneEventProgressStatus.SceneEventInProgress;
            }
            return NetworkManager.Singleton.SceneManager.LoadScene(sceneName, mode);
        }

        public static async Awaitable LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            Scene current = SceneManager.GetActiveScene();
            if (current.name == sceneName) return;

            // Server waits for all clients; clients wait for themselves.
            SceneEventType eventType = NetworkManager.Singleton.IsServer
                ? SceneEventType.LoadEventCompleted
                : SceneEventType.LoadComplete;

            await AwaitSceneEvent(eventType, sceneName, () => NetworkLoadScene(sceneName, mode));
        }

        public static async Awaitable UnloadSceneAsync(Scene scene)
        {
            if (!scene.isLoaded) return;

            await AwaitSceneEvent(SceneEventType.UnloadComplete, scene.name,
                () => NetworkManager.Singleton.SceneManager.UnloadScene(scene));
        }
    }
}