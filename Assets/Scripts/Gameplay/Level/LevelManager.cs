using System;
using System.Collections.Generic;
using System.Linq;
using Helteix.ChanneledProperties.Priorities;
using Helteix.Singletons.SceneServices;
using OverBang.GameName.Core;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.Database;
using OverBang.GameName.Core.Metrics;
using OverBang.GameName.Managers;
using OverBang.Pooling;
using OverBang.Pooling.Dependencies;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.Pool;

namespace OverBang.GameName.Gameplay
{
    public class LevelManager : SceneService<LevelManager>
    {
        public event Action<List<IPoolDependencyProvider>> OnCollectSceneProviders;
        public LevelState State { get; protected set; }
        
        private Dictionary<IPlayer, GameObject> currentPlayers;

        private GameplayPhase currentPhase;
        private GameplayPhase.GameplaySettings Settings => currentPhase.Settings;
        
        public virtual async Awaitable Initialize(GameplayPhase phase)
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

        public virtual void StartLevel()
        {
            State = LevelState.Running;
        }

        public virtual void Dispose()
        {
            if (State == LevelState.Disposed) return;
            
            PoolManager.Instance.ClearPools();
            
            State = LevelState.Disposed;
        }

        protected virtual async Awaitable SetupGameMap()
        {
            // Placeholder for map setup logic
            await Awaitable.EndOfFrameAsync();
        }
        
        protected virtual void SetupPlayer()
        {
            currentPlayers = new Dictionary<IPlayer, GameObject>();
            IPlayer currentPlayer = SessionManager.Global.CurrentPlayer;
            
            if (TryGetCharacterDataByPlayer(currentPlayer, out CharacterData characterData))
            {
                GameObject playerObject = Instantiate(characterData.CharacterPrefab);
                currentPlayers.Add(currentPlayer, playerObject);
            }
        }

        protected virtual async Awaitable SetupEnemies()
        {
            await Awaitable.EndOfFrameAsync();
        }

        protected virtual async Awaitable SetupUI()
        {
            await Awaitable.EndOfFrameAsync();
            GameController.CursorLockModePriority.AddPriority(this, PriorityTags.High, CursorLockMode.Locked);
            GameController.CursorVisibleStatePriority.AddPriority(this, PriorityTags.High, false);
        }
        
        private async Awaitable SetupPooling()
        {
            using (ListPool<IPoolDependencyProvider>.Get(out List<IPoolDependencyProvider> providers))
            {
                IPlayer currentPlayer = SessionManager.Global.CurrentPlayer;
                if (TryGetCharacterDataByPlayer(currentPlayer, out CharacterData characterData))
                {
                    providers.Add(characterData);
                }
                
                OnCollectSceneProviders?.Invoke(providers);

                PoolDependenciesCollector collector = new PoolDependenciesCollector();
                foreach (IPoolConfig config in collector.Collect(providers))
                    PoolManager.Instance.RegisterPool(config);
            }
            
            await Awaitable.MainThreadAsync();
        }

        private bool TryGetCharacterDataByPlayer(IPlayer player, out CharacterData characterData)
        {
            string characterDataPropertyName = ConstID.Global.PlayerPropertyCharacterData;
            return player.TryGetAssetByPlayerProperty(characterDataPropertyName, out characterData);
        }
    }
}