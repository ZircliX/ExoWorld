using System;
using System.Collections.Generic;
using System.Linq;
using Helteix.ChanneledProperties.Priorities;
using Helteix.Singletons.SceneServices;
using OverBang.GameName.Core;
using OverBang.GameName.Gameplay.Gameplay;
using OverBang.Pooling;
using OverBang.Pooling.Dependencies;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace OverBang.GameName.Gameplay
{
    public class LevelManager : SceneService<LevelManager>
    {
        public event Action<List<IPoolDependencyProvider>> OnCollectSceneProviders; 
        public LevelState State { get; protected set; }

        
        private Dictionary<PlayerProfile, GameObject> currentPlayers;

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
            await SetupPlayer();
            await SetupEnemies();
            await SetupUI();

            await SetupPooling();
            State = LevelState.Ready;
        }

        public virtual void StartLevel()
        {
            State = LevelState.Running;
        }

        public virtual async void Dispose()
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
        
        protected virtual async Awaitable SetupPlayer()
        {
            currentPlayers = new Dictionary<PlayerProfile, GameObject>();
            for (int i = 0; i < Settings.playerProfiles.Length; i++)
            {
                PlayerProfile profile = Settings.playerProfiles[i];
                currentPlayers.Add(profile, null);
                Addressables.InstantiateAsync(profile.characterData.CharacterPrefabRef).Completed +=
                    handle =>
                    {
                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            currentPlayers[profile] = handle.Result;
                        }
                    };
            }

            while (currentPlayers.Any(ctx => ctx.Value == null))
                await Awaitable.EndOfFrameAsync();
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
            using (ListPool<IPoolDependencyProvider>.Get(out var providers))
            {
                foreach (var profile in Settings.playerProfiles)
                    providers.Add(profile.characterData);
                
                OnCollectSceneProviders?.Invoke(providers);

                PoolDependenciesCollector collector = new PoolDependenciesCollector();
                foreach (var config in collector.Collect(providers))
                    PoolManager.Instance.RegisterPool(config);
            }
            
            await Awaitable.MainThreadAsync();
        }
    }
}