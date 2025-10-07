using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OverBang.Pooling;
using OverBang.Pooling.Resource;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class LevelManager : MonoBehaviour
    {
        public LevelState State { get; private set; }

        private LevelDefinition levelDefinition;
        private List<PoolConfigAsset> requiredPools;

        public IReadOnlyList<PoolConfigAsset> RequiredPools => requiredPools;
        
        public async Awaitable Initialize(LevelDefinition definition)
        {
            if (State != LevelState.None)
                throw new InvalidOperationException("LevelManager already initialized.");
            
            State = LevelState.None;
            requiredPools = new List<PoolConfigAsset>();

            levelDefinition = definition;
            State = LevelState.Initializing;

            CollectDependencies();

            PoolManager.Instance.RegisterPools(requiredPools.ToArray());

            await Task.CompletedTask;
        }

        public async Awaitable LoadContent()
        {
            State = LevelState.Loading;

            // TODO: load scene objects
            await Task.CompletedTask;
        }

        public void Run()
        {
            State = LevelState.Running;
        }

        public async Awaitable UnloadContent()
        {
            State = LevelState.Unloading;

            // TODO: unload scene objects
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            if (State == LevelState.Disposed) return;

            PoolManager.Instance.UnregisterPools(requiredPools.ToArray());
            requiredPools.Clear();

            State = LevelState.Disposed;
        }

        // -------- Dependency Resolution --------

        private void CollectDependencies()
        {
            requiredPools.Clear();

            if (levelDefinition.Equals(null)) return;

            // players
            foreach (CharacterPoolBinding binding in levelDefinition.Players)
                if (binding.PoolConfig != null)
                    requiredPools.Add(binding.PoolConfig);
            
            // extra FX
            requiredPools.AddRange(levelDefinition.ExtraPools);

            // remove duplicates
            requiredPools = requiredPools.Distinct().ToList();
        }
    }
}