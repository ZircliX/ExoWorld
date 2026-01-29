using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Characters;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public interface IAbility
    {
        ICaster Caster { get; }
        bool IsActive { get; }
        bool CanBeUsed { get; }

        void Begin();
        void Tick(float deltaTime);
        void End();
    }
    
    public interface IAbility<out TData> : IAbility 
        where TData : AbilityData
    {
        TData Data { get; }
    }
}