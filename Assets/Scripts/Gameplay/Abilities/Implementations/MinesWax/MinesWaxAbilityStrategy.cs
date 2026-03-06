using System.Collections.Generic;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.GameMode.Players;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public abstract class MinesWaxAbilityStrategy<TData> : IAbilityStrategy<MinesWaxData, TData>
        where TData : IMinesWaxStrategyData, IAbilityStrategyData
    {
        protected ICaster Caster { get; private set; }
        protected TData Data { get; private set; }
        protected List<MineWax> activeMines;
        
        private NetworkSpawnManager spawnManager;
        private LocalGamePlayer localPlayer;
        
        public void Initialize(IAbility<MinesWaxData> ability, ICaster caster, TData data)
        {
            Caster = caster;
            Data = data;
            activeMines = new List<MineWax>(3);
            
            spawnManager = NetworkManager.Singleton.SpawnManager;
            localPlayer = GamePlayerManager.Instance.GetLocalPlayer();
        }
        
        public virtual void Begin(IAbility<MinesWaxData> ability)
        {
            IExplosionStrategy explosionStrategy = GetExplosionStrategy();
            
            NetworkObject networkObject = spawnManager.InstantiateAndSpawn(ability.DataT.MineWaxPrefab, 
                localPlayer.ClientID,
                true,
                false,
                false,
                Caster.CastAnchor.position + Caster.Forward * 1.5f, Caster.transform.rotation);

            if (networkObject.TryGetComponent(out MineWax mine))
            {
                mine.Initialize(ability.DataT, Caster.Forward, explosionStrategy);
                activeMines.Add(mine);
            }
        }

        public virtual void Tick(IAbility<MinesWaxData> ability, float deltaTime)
        {
        }

        public virtual void End(IAbility<MinesWaxData> ability)
        {
        }

        public void Dispose(IAbility<MinesWaxData> ability)
        {
        }

        protected abstract IExplosionStrategy GetExplosionStrategy();
    }
    
    [CreateStrategyFor(typeof(MinesWaxStrategyData))]
    public class MinesWaxAbilityStrategy : MinesWaxAbilityStrategy<MinesWaxStrategyData>
    {
        protected override IExplosionStrategy GetExplosionStrategy()
        {
            return new StandardExplosion(Data.Damage);
        }
    }

    [CreateStrategyFor(typeof(MinesWaxCryoStrategyData))]
    public class MinesWaxCryoAbilityStrategy : MinesWaxAbilityStrategy<MinesWaxCryoStrategyData>
    {
        protected override IExplosionStrategy GetExplosionStrategy()
        {
            return new CryoExplosion(Data.Damage, Data.SlowDuration, Data.SlowPercentage);
        }
    }
    
    [CreateStrategyFor(typeof(MinesWaxNovaStrategyData))]
    public class MinesWaxNovaAbilityStrategy : MinesWaxAbilityStrategy<MinesWaxNovaStrategyData>
    {
        protected override IExplosionStrategy GetExplosionStrategy()
        {
            return new NovaExplosion(Data.Damage, Data.ExplosionInterval, Data.ExplosionCount);
        }
    }
}