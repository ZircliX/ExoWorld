using System.Threading;
using OverBang.GameName.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        
        public bool IsDone { get; private set; }

        public GameplayPhase(GameplaySettings gameplaySettings)
        {
            Settings = gameplaySettings;
        }

        protected virtual async Awaitable OnBegin()
        {
            await SessionManager.Global.CurrentPlayer.UpdatePlayerProperty(ConstID.Global.PlayerPropertyPhaseStatus, nameof(PhaseStatus.None));
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            LevelManager = CreateLevelManager();
            await LoadScene();
            
            if (LevelManager != null)
            {
                await LevelManager.Initialize(this);
                LevelManager.StartLevel();
            }
        }

        protected virtual async Awaitable Execute()
        {
            await AwaitableUtils.AwaitableUntil(() => IsDone, CancellationToken.None);
            
            CurrentEndInfos = new GameplayEndInfos()
            {
                isFinished = false,
                score = 1,
            };
        }
        
        protected virtual async Awaitable OnEnd()
        {
            await SessionManager.Global.CurrentPlayer.UpdatePlayerProperty(ConstID.Global.PlayerPropertyPhaseStatus, nameof(PhaseStatus.None));
            LevelManager.Dispose();
            Object.DestroyImmediate(LevelManager);
            
            await AwaitableUtils.CompletedAwaitable;
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
        protected abstract LevelManager CreateLevelManager();
        
        Awaitable IPhase.OnBegin() => OnBegin();
        Awaitable IPhase.OnEnd() => OnEnd();
        Awaitable IPhase.Execute() => Execute();

    }
}