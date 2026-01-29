using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Characters;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public abstract class PistoletHellaAbilityStrategy<TData> : IAbilityStrategy<PistoletHellaData, TData>
        where TData : IPistoletHellaAbilityStrategyData, IAbilityStrategyData
    {
        public void Initialize(IAbility<PistoletHellaData> ability, ICaster caster, TData data)
        {
        }
        
        public virtual void Begin(IAbility<PistoletHellaData> ability)
        {
        }

        public virtual void Tick(IAbility<PistoletHellaData> ability, float deltaTime)
        {
        }

        public virtual void End(IAbility<PistoletHellaData> ability)
        {
        }

        public void Dispose(IAbility<PistoletHellaData> ability)
        {
        }
    }

    [CreateStrategyFor(typeof(PistoletHellaStrategyData))]
    public class PistoletHellaAbilityStrategy : PistoletHellaAbilityStrategy<PistoletHellaStrategyData>
    {
        
    }

    [CreateStrategyFor(typeof(PistoletHellaAreaStrategyData))]
    public class PistoletHellaAreaAbilityStrategy : PistoletHellaAbilityStrategy<PistoletHellaAreaStrategyData>
    {
        
    }
    
    [CreateStrategyFor(typeof(PistoletHellaFrozenStrategyData))]
    public class PistoletHellaFrozenAbilityStrategy : PistoletHellaAbilityStrategy<PistoletHellaFrozenStrategyData>
    {
        
    }
}