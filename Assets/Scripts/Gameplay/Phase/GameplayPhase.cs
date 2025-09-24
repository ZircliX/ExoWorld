using System;
using System.Threading;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.GameAssets;
using OverBang.GameName.Core.Scene;
using OverBang.GameName.Gameplay.Gameplay.Listeners;
using OverBang.GameName.Managers;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Gameplay
{
    public class GameplayPhase
    {
        [System.Serializable]
        public struct GameplaySettings
        {
            public GameDatabase gameDatabase;
            public CharacterData playerCharacter;
        }

        [System.Serializable]
        public struct GameplayRewards
        {
            public int score; // test
        }
        
        public static async Awaitable<GameplayRewards> CreateAsync(GameplaySettings settings)
        {
            SceneReference gameSceneRef = SceneCollection.Global.GameSceneRef;
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            if (currentSceneName != gameSceneRef.Path)
            {
                Task loadSceneAsync = SceneLoader.LoadSceneAsync(gameSceneRef.Name);
                await loadSceneAsync;
            }

            GameplayListener[] listeners = UnityEngine.Object.FindObjectsByType<GameplayListener>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            GameplayPhase phase = new GameplayPhase(settings, listeners);

            await Awaitable.EndOfFrameAsync();
            
            await phase.Initialize();
            await AwaitableUtils.AwaitableUntil(() => phase.IsDone, CancellationToken.None);
            
            return phase.CurrentRewards;
        }
        
        public event Action<bool> OnCompleted;
        
        private readonly GameplaySettings settings;
        private readonly GameplayListener[] listeners;
        
        public event Action<CharacterData> OnSpawnPlayer;
        
        public GameplayRewards CurrentRewards { get; private set; }
        public bool IsDone { get; private set; }

        private GameplayPhase(GameplaySettings settings, GameplayListener[] listeners)
        {
            this.settings = settings;
            this.listeners = listeners;
        }
        
        public void CompletePhase(bool success)
        {
            for (int i = 0; i < listeners.Length; i++)
                listeners[i].OnRelease(this);
            
            IsDone = true;
            OnCompleted?.Invoke(success);
        }

        /*
            TODO:
             - Start game loop
             - Handle game end conditions
             - Collect rewards and stats
             - Transition back to hub or main menu
        */
        private async Awaitable Initialize()
        {
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i].current = this;
                listeners[i].OnInit(this);
            }
            
            await SetupGameMap();
            await SetupPlayer();
            await SetupEnemies();
            await SetupUI();

            CurrentRewards = new GameplayRewards()
            {
                score = 10
            };
        }

        private async Awaitable SetupGameMap()
        {
            // Placeholder for map setup logic
            await Awaitable.EndOfFrameAsync();
        }
        
        private async Awaitable SetupPlayer()
        {
            OnSpawnPlayer?.Invoke(settings.playerCharacter);
            Debug.Log( "[GameplayPhase] Initialized with player character: " + settings.playerCharacter?.AgentName);
            await Awaitable.EndOfFrameAsync();
        }

        private async Awaitable SetupEnemies()
        {
            await Awaitable.EndOfFrameAsync();
        }

        private async Awaitable SetupUI()
        {
            await Awaitable.EndOfFrameAsync();
        }
    }
}