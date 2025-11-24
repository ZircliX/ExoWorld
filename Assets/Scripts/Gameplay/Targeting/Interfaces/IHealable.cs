using System;

namespace OverBang.GameName.Gameplay
{
    public interface IHealable
    {
        event Action OnHealed;
        float Health { get; }
        float MaxHealth { get; }
        bool IsAlive { get; }
        void Heal(float amount);
    }
}