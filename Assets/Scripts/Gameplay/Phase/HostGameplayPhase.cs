using System.Threading;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using OverBang.GameName.Core.Scenes;
using OverBang.GameName.Core.Utils;
using OverBang.GameName.Managers;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class HostGameplayPhase : GameplayPhase
    {
        public HostGameplayPhase(GameplaySettings gameplaySettings) : base(gameplaySettings)
        {
        }

        protected override async Awaitable LoadScene()
        {
            SceneReference gameSceneRef = SceneCollection.Global.GameSceneRef;
            string currentSceneName = SceneLoader.GetCurrentSceneName();

            if (currentSceneName != gameSceneRef.Path)
            {
                bool loadingCompleted = false;
                
                void OnSceneEvent(SceneEvent sceneEvent)
                {
                    if (sceneEvent.SceneName == gameSceneRef.Name && sceneEvent.SceneEventType == SceneEventType.LoadComplete)
                    {
                        loadingCompleted = true;
                    }
                }

                NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
        
                try
                {
                    SceneLoader.NetworkLoadScene(gameSceneRef.Name);
                    await AwaitableUtils.AwaitableUntil(() => loadingCompleted == true, CancellationToken.None);
                }
                finally
                {
                    NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
                }
            }
        }

        protected override async Awaitable<LevelManager> CreateLevelManager()
        {
            GameObject levelManager = new GameObject("LevelManager")
            {
                hideFlags = HideFlags.NotEditable
            };
            
            LevelManager = levelManager.AddComponent(typeof(LevelManager)) as LevelManager;

            if (LevelManager != null)
            {
                await LevelManager.Initialize(this);
                LevelManager.StartLevel();
            }

            return LevelManager;
        }
    }
}