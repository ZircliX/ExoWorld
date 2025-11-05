
using OverBang.GameName.Core.Phases;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public abstract class GameplayPhase : IPhase
    {
        [System.Serializable]
        public struct GameplaySettings
        {
 
        }

        [System.Serializable]
        public struct GameplayEndInfos
        {
            public int score;
            public bool isFinished;
        }
        
        public readonly GameplaySettings Settings;
        
        public GameplayEndInfos CurrentEndInfos { get; protected set; }
        
        public LevelManager LevelManager { get; protected set; }

        public GameplayPhase(GameplaySettings gameplaySettings)
        {
            Settings = gameplaySettings;
        }

        public async Awaitable OnBegin()
        {
            await LoadScene();
            await CreateLevelManager();
            
            // sétè tro dur mé ct coul
            // NetworkManager.Singleton.PrefabHandler.AddHandler(15, new CustomNetworkPrefabHandler())
        }

        public Awaitable OnEnd(bool success)
        {
            return null;
        }

        protected abstract Awaitable LoadScene();
        protected abstract Awaitable<LevelManager> CreateLevelManager();
    }
}