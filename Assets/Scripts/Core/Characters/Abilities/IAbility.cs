using UnityEngine;

namespace OverBang.GameName.Core
{
    public interface IAbility
    {
        bool IsActive { get; }
        CooldownComponent Cooldown { get; }
        bool CanBeUsed => !IsActive && Cooldown.IsReady;

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