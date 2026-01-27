using Eflatun.SceneReference;
using OverBang.ExoWorld.Core.Scene;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OverBang.ExoWorld.Gameplay.Phase
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

    }
}