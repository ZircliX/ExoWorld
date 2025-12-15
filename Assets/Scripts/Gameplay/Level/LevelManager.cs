using System;
using Helteix.ChanneledProperties.Priorities;
using OverBang.GameName.Core;
using OverBang.Pooling;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Gameplay
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
            DontDestroyOnLoad(gameObject);
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
            SetupPlayer();
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
                
                Vector3 position = new Vector3(11f, 1f, 24f);
                Quaternion rotation = Quaternion.Euler(0f, 180f, 0f);
                NetworkObject player = PlayerSpawner.SpawnPlayerObject(characterData, clientID, position, rotation);
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

        private void SetState(LevelState state)
        {
            State = state;
            OnStateChanged?.Invoke(state);
        }
    }
}