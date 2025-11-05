using Eflatun.SceneReference;
using OverBang.GameName.Core.Scenes;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class HostGameplayPhase : GameplayPhase
    {
        public HostGameplayPhase(GameplaySettings gameplaySettings) : base(gameplaySettings)
        {
        }

        protected override Awaitable LoadScene()
        {
            SceneReference gameSceneRef = SceneCollection.Global.GameSceneRef;
            string currentSceneName = SceneLoader.GetCurrentSceneName();

            if (currentSceneName != gameSceneRef.Path)
            {
                SceneEventProgressStatus sceneProgress = SceneLoader.NetworkLoadScene(currentSceneName);
            }

            return null;
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