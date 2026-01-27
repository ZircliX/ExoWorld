using OverBang.ExoWorld.Core.Characters;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public abstract class AppareilZetaAbilityStrategy<TData> : IAbilityStrategy<AppareilZetaData, TData>
        where TData : IAppareilZetaAbilityStrategyData, IAbilityStrategyData
    {
        public void Initialize(IAbility<AppareilZetaData> ability, ICaster caster, TData data)
        {
        }
        
        public virtual void Begin(IAbility<AppareilZetaData> ability)
        {
        }

        public virtual void Tick(IAbility<AppareilZetaData> ability, float deltaTime)
        {
        }

        public virtual void End(IAbility<AppareilZetaData> ability)
        {
        }

        public void Dispose(IAbility<AppareilZetaData> ability)
        {
        }
    }
    
    [CreateStrategyFor(typeof(AppareilZetaStrategyData))]
    public class AppareilZetaAbilityStrategy : AppareilZetaAbilityStrategy<AppareilZetaStrategyData>
    {
        
    }
    
    [CreateStrategyFor(typeof(AppareilZetaPersistantStrategyData))]
    public class AppareilZetaPersistantAbilityStrategy: AppareilZetaAbilityStrategy<AppareilZetaPersistantStrategyData>
    {
        
    }
    
    [CreateStrategyFor(typeof(AppareilZetaCompressedStrategyData))]
    public class AppareilZetaCompressedAbilityStrategy : AppareilZetaAbilityStrategy<AppareilZetaCompressedStrategyData>
    {
        
    }
}