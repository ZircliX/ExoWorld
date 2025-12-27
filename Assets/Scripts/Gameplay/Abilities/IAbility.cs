using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public interface IAbility
    {
        bool IsActive { get; }
        float CooldownTimer { get; }

        void Begin();
        void OnTick(float deltaTime);
        void End();
    }

    public interface IAbility<out TData> : IAbility
        where TData : AbilityData
    {
        TData TypedData { get; }
        GameObject Owner { get; }

        bool Initialize(AbilityData data, GameObject owner);
    }
}