using System;
using System.Threading.Tasks;
using Helteix.ChanneledProperties.Priorities;
using OverBang.ExoWorld.Core;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Phases;
using OverBang.ExoWorld.Core.Utils;
using OverBang.ExoWorld.Gameplay.Enemies;
using OverBang.ExoWorld.Gameplay.Phase;
using OverBang.Pooling;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Level
{
    public sealed class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }
        public LevelState State { get; private set; }
        public event Action<LevelState> OnStateChanged; 
        
        private GameplayPhase currentPhase;
        private GameplayPhase.GameplaySettings Settings => currentPhase.Settings;
        public EnemySpawnerManager EnemySpawnerManager { get; private set; }

        private void Awake()
        {
            Instance = this;
            EnemySpawnerManager = new EnemySpawnerManager();
            EnemySpawnerManager.Register();
        }
        
        private void OnDisable()
        {
            EnemySpawnerManager.Unregister();
        }

        public async Awaitable Initialize(GameplayPhase phase)
        {
            if (State != LevelState.None)
            {
                Debug.LogError("LevelManager already initialized.");
                return;
            }
            
            SetState(LevelState.Initializing);
            currentPhase = phase;

            await SetupGameMap();
            SetupPlayer(GamePlayerManager.Instance.GetLocalPlayer());
            await SetupEnemies();
            await SetupPooling();
            await SetupUI();
            
            SetState(LevelState.Ready);
        }

        public void StartLevel()
        {
            SetState(LevelState.Running);
        }

        public void Dispose()
        {
            if (State == LevelState.Disposed) return;
            
            PoolManager.Instance.ClearPools();
            
            SetState(LevelState.Disposed);
        }

        private async Awaitable SetupGameMap()
        {
            await Task.CompletedTask;
        }

        private void SetupPlayer(LocalGamePlayer gamePlayer)
        {
            Transform spawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;
            gamePlayer.Spawn(spawnPoint.position, spawnPoint.rotation);
        }

        private async Awaitable SetupEnemies()
        {
            await Task.CompletedTask;
        }

        private async Awaitable SetupUI()
        {
            GameController.CursorLockModePriority.AddPriority(this, PriorityTags.High, CursorLockMode.Locked);
            GameController.CursorVisibleStatePriority.AddPriority(this, PriorityTags.High, false);
            
            await Task.CompletedTask;
        }
        
        private async Awaitable SetupPooling()
        {
            await PoolUtils.SetupPooling(PoolUtils.PoolType.All);
        }

        private void SetState(LevelState state)
        {
            State = state;
            OnStateChanged?.Invoke(state);
        }
    }
}