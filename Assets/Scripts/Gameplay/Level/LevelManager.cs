using Helteix.ChanneledProperties.Priorities;
using Helteix.Singletons.SceneServices;
using OverBang.GameName.Core;
using OverBang.Pooling;
using OverBang.Pooling.Resource;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public sealed class LevelManager : SceneService<LevelManager>
    {
        public LevelState State { get; private set; }
        
        private GameplayPhase currentPhase;
        private GameplayPhase.GameplaySettings Settings => currentPhase.Settings;
        public EnemySpawnerManager EnemySpawnerManager { get; private set; }

        protected override void Activate()
        {
            EnemySpawnerManager = new EnemySpawnerManager();
            EnemySpawnerManager.Register();
        }
        
        protected override void Deactivate()
        {
            base.Deactivate();
            EnemySpawnerManager.Unregister();
        }

        public async Awaitable Initialize(GameplayPhase phase)
        {
            if (State != LevelState.None)
            {
                Debug.LogError("LevelManager already initialized.");
                return;
            }
            
            State = LevelState.Initializing;
            currentPhase = phase;

            await SetupGameMap();
            SetupPlayer();
            await SetupEnemies();
            await SetupUI();
            await SetupPooling();
            
            State = LevelState.Ready;
        }

        public void StartLevel()
        {
            State = LevelState.Running;
        }

        public void Dispose()
        {
            if (State == LevelState.Disposed) return;
            
            PoolManager.Instance.ClearPools();
            
            State = LevelState.Disposed;
        }

        private async Awaitable SetupGameMap()
        {
            await AwaitableUtils.CompletedAwaitable;
        }

        private void SetupPlayer()
        {
            IPlayer currentPlayer = SessionManager.Global.CurrentPlayer;
            //Debug.Log("SetupPlayer : " + currentPlayer.Id);

            if (currentPlayer.TryGetCharacterDataByPlayer(out CharacterData characterData))
            {
                ulong clientID = NetworkManager.Singleton.LocalClient.ClientId;
                //Debug.Log($"Player {clientID} has CharacterData {characterData.AgentName}");
                NetworkObject player = PlayerSpawner.SpawnPlayerObject(characterData, clientID, SessionManager.Global.CurrentPlayer);
            }
            else
            {
                Debug.LogError($"Player {currentPlayer.Id} does not have CharacterData");
            }
        }

        private async Awaitable SetupEnemies()
        {
            await AwaitableUtils.CompletedAwaitable;
        }

        private async Awaitable SetupUI()
        {
            GameController.CursorLockModePriority.AddPriority(this, PriorityTags.High, CursorLockMode.Locked);
            GameController.CursorVisibleStatePriority.AddPriority(this, PriorityTags.High, false);
            await AwaitableUtils.CompletedAwaitable;
        }
        
        private async Awaitable SetupPooling()
        {
            await PoolUtils.SetupPooling();
        }
    }
}