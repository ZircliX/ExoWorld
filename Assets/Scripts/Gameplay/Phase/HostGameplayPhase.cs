using Eflatun.SceneReference;
using OverBang.GameName.Core;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            Scene currentSceneName = SceneLoader.GetCurrentScene();
    
            if (currentSceneName.name != gameSceneRef.Name && NetworkManager.Singleton.IsServer)
            {
                await SceneLoader.LoadSceneAsync(gameSceneRef.Name, LoadSceneMode.Single);
            }
        }

        protected override async Awaitable<LevelManager> CreateLevelManager()
        {
            GameObject levelManager = new GameObject("LevelManager")
            {
                hideFlags = HideFlags.NotEditable
            };
            
            LevelManager = levelManager.AddComponent<LevelManager>();

            if (LevelManager != null)
            {
                await LevelManager.Initialize(this);
                LevelManager.StartLevel();
            }

            return LevelManager;
        }
    }
}