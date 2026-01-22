using OverBang.ExoWorld.Core;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class GraineDeChanuiAbilityStrategy<TData> : IAbilityStrategy<GraineDeChanuiData, TData>
        where TData : IGraineDeChanuiAbilityStrategyData, IAbilityStrategyData
    {
        public void Initialize(IAbility<GraineDeChanuiData> ability, ICaster caster, TData data)
        {
        }
        
        public virtual void Begin(IAbility<GraineDeChanuiData> ability)
        {
        }

        public virtual void Tick(IAbility<GraineDeChanuiData> ability, float deltaTime)
        {
        }

        public virtual void End(IAbility<GraineDeChanuiData> ability)
        {
        }

        public void Dispose(IAbility<GraineDeChanuiData> ability)
        {
        }
    }
    
    [CreateStrategyFor(typeof(GraineDeChanuiStrategyData))]
    public class GraineDeChanuiAbilityStrategy : GraineDeChanuiAbilityStrategy<GraineDeChanuiStrategyData>
    {
        
    }
    
    [CreateStrategyFor(typeof(GraineDeChanuiChaotiquesStrategyData))]
    public class GraineDeChanuiChaotiquesAbilityStrategy : GraineDeChanuiAbilityStrategy<GraineDeChanuiChaotiquesStrategyData>
    {
        
    }
    
    [CreateStrategyFor(typeof(GraineDeChanuiSoporifiquesStrategyData))]
    public class GraineDeChanuiSoporifiquesAbilityStrategy : GraineDeChanuiAbilityStrategy<GraineDeChanuiSoporifiquesStrategyData>
    {
        
    }
}