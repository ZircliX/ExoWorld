using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using OverBang.GameName.Core;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.GameAssets;
using OverBang.GameName.Core.Scenes;
using OverBang.GameName.Gameplay.Gameplay.Listeners;
using OverBang.GameName.Managers;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class GameplayPhase
    {
        public static event Action<GameplayPhase> OnNewPhaseBegins; 
        public static event Action<GameplayPhase> OnNewPhaseEnds;
        
        [System.Serializable]
        public struct GameplaySettings
        {
            public Type levelManagerType;
            public GameDatabase gameDatabase;
            public PlayerProfile[] playerProfiles;
        }

        [System.Serializable]
        public struct GameplayEndInfos
        {
            public int score;
            public bool isFinished;
        }
        
        public static async Awaitable<GameplayEndInfos> CreateAsync(GameplaySettings settings)
        {
            SceneReference gameSceneRef = SceneCollection.Global.GameSceneRef;
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            if (currentSceneName != gameSceneRef.Path)
                await SceneLoader.LoadSceneAsync(gameSceneRef.Name, setActive: true);

            GameplayPhase phase = new GameplayPhase(settings);
            await Awaitable.EndOfFrameAsync();
            OnNewPhaseBegins?.Invoke(phase);
            
            bool success = await phase.Initialize();
            if (!success)
                return new GameplayEndInfos() { isFinished = true, score = 0 };
            
            await AwaitableUtils.AwaitableUntil(() => phase.IsDone, CancellationToken.None);
            
            OnNewPhaseEnds?.Invoke(phase);
            return phase.CurrentEndInfos;
        }
        
        public event Action<bool> OnCompleted;
        
        public readonly GameplaySettings Settings;
        
        public GameplayEndInfos CurrentEndInfos { get; private set; }
        public bool IsDone { get; private set; }
        
        public LevelManager LevelManager { get; private set; }

        private GameplayPhase(GameplaySettings gameplaySettings)
        {
            Settings = gameplaySettings;
        }

        public void CompletePhase(bool success)
        {
            IsDone = true;
            LevelManager.Dispose();
            OnCompleted?.Invoke(success);
        }

        /*
            TODO:
             - Handle game end conditions
             - Collect rewards and stats
        */
        private async Awaitable<bool> Initialize()
        {
            await Settings.gameDatabase.ChangeCatalog(new DatabaseCatalog()
            {
                name = "Gameplay catalog",
                assetsKeys = new List<object>(),
                labels = new List<string>()
            });
            
            GameObject levelManager = new GameObject("LevelManager")
            {
                hideFlags = HideFlags.NotEditable
            };
            
            LevelManager = levelManager.AddComponent(Settings.levelManagerType ?? typeof(LevelManager)) as LevelManager;

            if (LevelManager != null)
            {
                await LevelManager.Initialize(this);
                LevelManager.StartLevel();
                return true;
            }

            return false;
        }
    }
}