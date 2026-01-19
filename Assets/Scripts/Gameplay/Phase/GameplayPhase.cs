using System.Threading;
using System.Threading.Tasks;
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
            public GameplayRewards rewards;
            public bool isFinished;
        }

        [System.Serializable]
        public struct GameplayRewards
        {
            public int trinititeReward;
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
                rewards = new GameplayRewards()
                {
                    trinititeReward = PlayerInventory.Trinitite,
                }
            };
        }
        
        protected virtual async Awaitable OnEnd()
        {
            await SessionManager.Global.CurrentPlayer.UpdatePlayerProperty(ConstID.Global.PlayerPropertyPhaseStatus, nameof(PhaseStatus.None));
            LevelManager.Dispose();
            Object.DestroyImmediate(LevelManager);
            
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
        protected abstract LevelManager CreateLevelManager();
        
        Awaitable IPhase.OnBegin() => OnBegin();
        Awaitable IPhase.OnEnd() => OnEnd();
        Awaitable IPhase.Execute() => Execute();

    }
}