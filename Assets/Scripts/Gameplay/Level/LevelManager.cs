using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OverBang.Pooling;
using OverBang.Pooling.Resource;

namespace OverBang.GameName.Gameplay
{
    public class LevelManager
    {
        public LevelState State { get; private set; }

        private LevelDefinition levelDefinition;
        private List<PoolConfigAsset> requiredPools;

        public IReadOnlyList<PoolConfigAsset> RequiredPools => requiredPools;

        public LevelManager()
        {
            State = LevelState.None;
            requiredPools = new List<PoolConfigAsset>();
        }
        
        public async Task Initialize(LevelDefinition definition)
        {
            if (State != LevelState.None)
                throw new InvalidOperationException("LevelManager already initialized.");

            levelDefinition = definition;
            State = LevelState.Initializing;

            CollectDependencies();

            PoolManager.Instance.RegisterPools(requiredPools.ToArray());

            await Task.CompletedTask;
        }

        public async Task LoadContent()
        {
            State = LevelState.Loading;

            // TODO: load scene objects
            await Task.CompletedTask;
        }

        public void Run()
        {
            State = LevelState.Running;
        }

        public async Task UnloadContent()
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