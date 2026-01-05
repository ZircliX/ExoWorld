using OverBang.GameName.Gameplay;

namespace OverBang.GameName.Core
{
    public interface IAbility
    {
        IAbilityCaster Caster { get; }
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