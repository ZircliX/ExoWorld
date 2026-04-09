using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Characters;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public interface IAbility
    {
        ICaster Caster { get; }
        bool IsActive { get; }
        bool CanBeUsed { get; }
        AbilityData Data { get; }
        float Duration { get; }
        
        Action<IAbility> OnAbilityEnded{ get; set; }
        Action<IAbility> OnAbilityCooldownEnded{ get; set; }

        void Begin();
        void Tick(float deltaTime);
        void End();
        void SetDuration(float duration);
    }
    
    public interface IAbility<out TData> : IAbility 
        where TData : AbilityData
    {
        TData DataT { get; }
    }
}