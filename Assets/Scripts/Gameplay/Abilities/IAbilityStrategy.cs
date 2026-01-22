using OverBang.ExoWorld.Core;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public interface IAbilityStrategy<in TData> where TData : AbilityData
    {
        void Initialize(IAbility<TData> ability, ICaster caster, IAbilityStrategyData data);
        
        void Begin(IAbility<TData> ability);
        void Tick(IAbility<TData> ability, float deltaTime);
        void End(IAbility<TData> ability);
        
        void Dispose(IAbility<TData> ability);
    }
    
    public interface IAbilityStrategy<in TData, in TStrategyData> : IAbilityStrategy<TData> 
        where TData : AbilityData
        where TStrategyData : IAbilityStrategyData
    {
        void IAbilityStrategy<TData>.Initialize(IAbility<TData> ability, ICaster caster, IAbilityStrategyData data)
        {
            if (data is TStrategyData typedData)
            {
                Initialize(ability, caster, typedData);
            }
        }
        
        void Initialize(IAbility<TData> ability, ICaster caster, TStrategyData data);
    }
}