using System.Collections.Generic;
using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public abstract class MinesWaxAbilityStrategy<TData> : IAbilityStrategy<MinesWaxData, TData>
        where TData : IMinesWaxStrategyData, IAbilityStrategyData
    {
        protected IAbilityCaster Caster { get; private set; }
        protected TData Data { get; private set; }
        protected List<MineWax> activeMines;
        
        public void Initialize(IAbility<MinesWaxData> ability, IAbilityCaster caster, TData data)
        {
            Caster = caster;
            Data = data;
            activeMines = new List<MineWax>(3);
        }
        
        public virtual void Begin(IAbility<MinesWaxData> ability)
        {
            IMineExplosionStrategy explosionStrategy = GetExplosionStrategy();
            
            MineWax mine = Object.Instantiate(ability.Data.MineWaxPrefab, Caster.transform.position + Caster.Forward * 3.5f, Quaternion.identity);
            mine.Initialize(ability.Data, Caster.Forward, explosionStrategy);
            
            activeMines.Add(mine);
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

        protected abstract IMineExplosionStrategy GetExplosionStrategy();
    }
    
    [CreateStrategyFor(typeof(MinesWaxStrategyData))]
    public class MinesWaxAbilityStrategy : MinesWaxAbilityStrategy<MinesWaxStrategyData>
    {
        protected override IMineExplosionStrategy GetExplosionStrategy()
        {
            return new StandardExplosion(Data.Damage);
        }
    }

    [CreateStrategyFor(typeof(MinesWaxCryoStrategyData))]
    public class MinesWaxCryoAbilityStrategy : MinesWaxAbilityStrategy<MinesWaxCryoStrategyData>
    {
        protected override IMineExplosionStrategy GetExplosionStrategy()
        {
            return new CryoExplosion(Data.Damage, Data.SlowDuration, Data.SlowPercentage);
        }
    }
    
    [CreateStrategyFor(typeof(MinesWaxNovaStrategyData))]
    public class MinesWaxNovaAbilityStrategy : MinesWaxAbilityStrategy<MinesWaxNovaStrategyData>
    {
        protected override IMineExplosionStrategy GetExplosionStrategy()
        {
            return new NovaExplosion(Data.Damage, Data.ExplosionInterval, Data.ExplosionCount);
        }
    }
}