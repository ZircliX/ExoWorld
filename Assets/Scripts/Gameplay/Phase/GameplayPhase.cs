using System.Threading;
using System.Threading.Tasks;
using OverBang.ExoWorld.Core.Menus;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Core.Phases;
using OverBang.ExoWorld.Core.Utils;
using OverBang.ExoWorld.Gameplay.Level;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OverBang.ExoWorld.Gameplay.Phase
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
            public bool isFinished;
        }

        protected readonly CancellationTokenSource cts;
        
        public readonly GameplaySettings Settings;
        
        public GameplayEndInfos CurrentEndInfos { get; protected set; }
        public LevelManager LevelManager { get; protected set; }
        
        public bool IsDone { get; private set; }

        public GameplayPhase(GameplaySettings gameplaySettings, CancellationTokenSource cts)
        {
            this.cts = cts;
            Settings = gameplaySettings;
        }

        protected virtual async Awaitable OnBegin()
        {
            await SessionManager.Global.CurrentPlayer.UpdatePlayerProperty(ConstID.Global.PlayerPropertyPhaseStatus, nameof(PhaseStatus.None));
            SceneManager.sceneLoaded += OnSceneLoaded;
            LevelManager = CreateLevelManager();
            
            await LoadingScreen.Instance.Show();

            await LoadScene();
            
            if (LevelManager != null)
            {
                await LevelManager.Initialize(this);
                LevelManager.StartLevel();
            }
            
            await LoadingScreen.Instance.Hide();
        }

        protected virtual async Awaitable Execute()
        {
            await AwaitableUtils.AwaitableUntil(() => IsDone, cts.Token);
            
            CurrentEndInfos = new GameplayEndInfos()
            {
                isFinished = false
            };
        }
        
        protected virtual async Awaitable OnEnd()
        {
            await SessionManager.Global.CurrentPlayer.UpdatePlayerProperty(ConstID.Global.PlayerPropertyPhaseStatus, nameof(PhaseStatus.None));
            LevelManager.Dispose();
            if(Application.isPlaying)
                Object.Destroy(LevelManager.gameObject);
            else
                Object.DestroyImmediate(LevelManager.gameObject);
            
            await Task.CompletedTask;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (scene.name == GameMetrics.Global.SceneCollection.GameSceneRef.Name)
            {
                Awaitable aw = SessionManager.Global.CurrentPlayer.UpdatePlayerProperty(ConstID.Global.PlayerPropertyPhaseStatus, nameof(PhaseStatus.SceneLoaded));
                aw.Run();
            }
        }

        public void SetIsDone() => IsDone = true;
        
        protected abstract Awaitable LoadScene();

        protected LevelManager CreateLevelManager()
        {
            //Debug.Log("Creating level manager");
            
            GameObject levelManager = new GameObject("LevelManager")
            {
                hideFlags = HideFlags.NotEditable
            };
            
            LevelManager = levelManager.AddComponent<LevelManager>();

            Object.DontDestroyOnLoad(LevelManager);
            return LevelManager;
        }
        
        Awaitable IPhase.OnBegin() => OnBegin();
        Awaitable IPhase.OnEnd() => OnEnd();
        Awaitable IPhase.Execute() => Execute();
    }
}