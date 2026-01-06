using OverBang.GameName.Core;

namespace OverBang.GameName.Gameplay
{
    public class MinesWaxAbilityStrategy<TData> : IAbilityStrategy<MinesWaxData, TData>
        where TData : IMinesWaxStrategyData, IAbilityStrategyData
    {
        public void Initialize(IAbility<MinesWaxData> ability, IAbilityCaster caster, TData data)
        {
        }
        
        public virtual void Begin(IAbility<MinesWaxData> ability)
        {
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
    }
    
    [CreateStrategyFor(typeof(MinesWaxStrategyData))]
    public class MinesWaxAbilityStrategy : MinesWaxAbilityStrategy<MinesWaxStrategyData>
    {
        
    }

    [CreateStrategyFor(typeof(MinesWasCryoAbilityStrategy))]
    public class MinesWasCryoAbilityStrategy : MinesWaxAbilityStrategy<MinesWaxCryoStrategyData>
    {
        
    }
    
    [CreateStrategyFor(typeof(MinesWasNovaAbilityStrategy))]
    public class MinesWasNovaAbilityStrategy : MinesWaxAbilityStrategy<MinesWaxNovaStrategyData>
    {
        
    }
}