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

namespace OverBang.GameName.Gameplay
{
    public sealed class LevelManager : SceneService<LevelManager>
    {
        public event Action<List<IPoolDependencyProvider>> OnCollectSceneProviders;
        public LevelState State { get; private set; }
        
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
            Debug.Log("SetupPlayer : " + currentPlayer.Id);

            if (currentPlayer.TryGetCharacterDataByPlayer(out CharacterData characterData))
            {
                ulong clientID = NetworkManager.Singleton.LocalClient.ClientId;
                NetworkObject player = PlayerSpawner.SpawnPlayerObject(characterData, clientID, SessionManager.Global.CurrentPlayer);
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
            await PoolUtils.SetupPooling(OnCollectSceneProviders);
        }
    }
}