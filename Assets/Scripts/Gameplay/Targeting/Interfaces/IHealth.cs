using System;

namespace OverBang.GameName.Gameplay
{
    public interface IHealth
    {
        event Action<float, float> OnHealthChanged;
        
        float Health { get; }
        float MaxHealth { get; }
        bool IsAlive { get; }
    }
}