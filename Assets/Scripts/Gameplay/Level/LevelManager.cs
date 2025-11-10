using System;
using System.Collections.Generic;
using Helteix.ChanneledProperties.Priorities;
using Helteix.Singletons.SceneServices;
using OverBang.GameName.Core;
using OverBang.Pooling;
using OverBang.Pooling.Dependencies;
using OverBang.Pooling.Resource;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.Pool;

namespace OverBang.GameName.Gameplay
{
    public class LevelManager : SceneService<LevelManager>
    {
        public event Action<List<IPoolDependencyProvider>> OnCollectSceneProviders;
        public LevelState State { get; protected set; }
        
        private Dictionary<IPlayer, NetworkObject> currentPlayers;

        private GameplayPhase currentPhase;
        private GameplayPhase.GameplaySettings Settings => currentPhase.Settings;

        protected override void Activate()
        {
            base.Activate();
            PoolManager.Instance.OnPoolAssetRegistered += OnPoolAssetRegistered;
            PoolManager.Instance.OnPoolAssetUnregistered += OnPoolAssetUnregistered;
        }

        protected override void Deactivate()
        {
            base.Deactivate();
            PoolManager.Instance.OnPoolAssetRegistered -= OnPoolAssetRegistered;
            PoolManager.Instance.OnPoolAssetUnregistered -= OnPoolAssetUnregistered;
        }
        
        private void OnPoolAssetRegistered(PoolResource resource)
        {
            switch (resource.Asset)
            {
                case PrefabPoolAsset prefabPoolAsset:
                    PoolingNetworkPrefabHandler networkPrefabHandler = new PoolingNetworkPrefabHandler(resource);
                    NetworkManager.Singleton.PrefabHandler.AddHandler(prefabPoolAsset.Prefab, networkPrefabHandler);
                    break;
            }
        }
        private void OnPoolAssetUnregistered(PoolResource resource)
        {
            switch (resource.Asset)
            {
                case PrefabPoolAsset prefabPoolAsset:
                    NetworkManager.Singleton.PrefabHandler.RemoveHandler(prefabPoolAsset.Prefab);
                    break;
            }
        }

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
            currentPlayers = new Dictionary<IPlayer, NetworkObject>();
            IPlayer currentPlayer = SessionManager.Global.CurrentPlayer;

            if (currentPlayer.TryGetCharacterDataByPlayer(out CharacterData characterData))
            {
                ulong clientID = NetworkManager.Singleton.LocalClient.ClientId;
                NetworkObject playerObject = Instantiate(GameMetrics.Global.PlayerControllerPrefab);
                playerObject.SpawnAsPlayerObject(clientID, destroyWithScene: true);

                if (playerObject.TryGetComponent(out IPlayerController playerController))
                {
                    playerController.SetDataRpc(characterData.ID);
                }
                
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
                if (currentPlayer.TryGetCharacterDataByPlayer(out CharacterData characterData))
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
    }
}