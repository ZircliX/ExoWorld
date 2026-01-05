using OverBang.GameName.Core;

namespace OverBang.GameName.Gameplay
{
    public abstract class DashAbilityStrategy<TData> : IAbilityStrategy<DashData, TData>
        where TData : IDashAbilityStrategyData, IAbilityStrategyData
    {
        protected PlayerMovement PlayerMovement { get; private set; }
        protected TData Data { get; private set; }
        
        public void Initialize(IAbility<DashData> ability, IAbilityCaster caster, TData data)
        {
            if (caster.gameObject.TryGetComponent(out PlayerMovement pm))
            {
                PlayerMovement = pm;
            }

            Data = data;
        }

        public virtual void Begin(IAbility<DashData> ability)
        {
        }

        public virtual void Tick(IAbility<DashData> ability, float deltaTime)
        {
        }

        public virtual void End(IAbility<DashData> ability)
        {
        }

        public void Dispose(IAbility<DashData> ability)
        {
        }
    }

    [CreateStrategyFor(typeof(AssaultDashStrategyData))]
    public class AssaultDashAbilityStrategy : DashAbilityStrategy<AssaultDashStrategyData>
    {
        
    }
    
    [CreateStrategyFor(typeof(ElectricDashStrategyData))]
    public class ElectricDashAbilityStrategy : DashAbilityStrategy<ElectricDashStrategyData>
    {
        
    }
    
    [CreateStrategyFor(typeof(SpectralDashStrategyData))]
    public class SpectralDashAbilityStrategy : DashAbilityStrategy<SpectralDashStrategyData>
    {
        
    }
}