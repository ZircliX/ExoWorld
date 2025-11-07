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
            LevelManager = await CreateLevelManager();
        }

        public async Awaitable OnEnd(bool success)
        {
            LevelManager.Dispose();
            
            CurrentEndInfos = new GameplayEndInfos()
            {
                isFinished = false,
                score = success ? 1 : 0,
            };
        }

        protected abstract Awaitable LoadScene();
        protected abstract Awaitable<LevelManager> CreateLevelManager();
    }
}