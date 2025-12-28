using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public interface IAbility
    {
        bool IsActive { get; }
        CooldownComponent Cooldown { get; }

        void Begin();
        void Tick(float deltaTime);
        void End();
    }

    public interface IAbility<out TData> : IAbility
        where TData : AbilityData
    {
        TData Data { get; }
        GameObject Owner { get; }
    }
}