using System;
using System.Threading;
using Ami.BroAudio;
using OverBang.ExoWorld.Core.Utils;
using OverBang.Pooling;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OverBang.ExoWorld.Core.Scene
{
    public static class SceneLoader
    {
        public static event Action OnPreSceneLoad;
        public static event Action OnPostSceneLoad;
        
        public static UnityEngine.SceneManagement.Scene GetCurrentScene() => SceneManager.GetActiveScene();
        
        static async Awaitable AwaitSceneEvent(SceneEventType eventType, string sceneName, Func<SceneEventProgressStatus> op)
        {
            bool done = false;

            void Handler(SceneEvent e)
            {
                // Some events report name via e.Scene or e.SceneName; handle both safely.
                string name = string.IsNullOrEmpty(e.SceneName) ? e.Scene.name : e.SceneName;
                if (name == sceneName && e.SceneEventType == eventType) done = true;
            }

            PoolManager.Instance.ClearPools();
            OnPreSceneLoad?.Invoke();
            BroAudio.Stop(BroAudioType.All, 1f);
            NetworkSceneManager sm = NetworkManager.Singleton.SceneManager;
            sm.OnSceneEvent += Handler;
            try
            {
                op();
                await AwaitableUtils.AwaitableUntil(() => done, CancellationToken.None);
                OnPostSceneLoad?.Invoke();
                await Awaitable.EndOfFrameAsync();
            }
            finally
            {
                sm.OnSceneEvent -= Handler;
            }
        }
        
        public static SceneEventProgressStatus NetworkLoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (!NetworkManager.Singleton.IsListening)
            {
                Debug.LogWarning($"NetworkLoadScene ignored on client: {sceneName}");
                return SceneEventProgressStatus.SceneEventInProgress;
            }
            return NetworkManager.Singleton.SceneManager.LoadScene(sceneName, mode);
        }
        
        public static async Awaitable LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            UnityEngine.SceneManagement.Scene current = SceneManager.GetActiveScene();
            if (current.name == sceneName) return;

            // Server waits for all clients; clients wait for themselves.
            SceneEventType eventType = NetworkManager.Singleton.IsServer
                ? SceneEventType.LoadEventCompleted
                : SceneEventType.LoadComplete;

            await AwaitSceneEvent(eventType, sceneName, () => NetworkLoadScene(sceneName, mode));
        }

        public static async Awaitable UnloadSceneAsync(UnityEngine.SceneManagement.Scene scene)
        {
            if (!scene.isLoaded) return;

            await AwaitSceneEvent(SceneEventType.UnloadComplete, scene.name,
                () => NetworkManager.Singleton.SceneManager.UnloadScene(scene));
        }
    }
}