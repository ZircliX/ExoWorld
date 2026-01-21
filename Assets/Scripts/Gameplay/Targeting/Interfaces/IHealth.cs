using System;

namespace OverBang.ExoWorld.Gameplay
{
    public interface IHealth
    {
        event Action<float, float> OnHealthChanged;
        
        float MinHealth { get; }
        float Health { get; }
        float MaxHealth { get; }
        bool IsAlive { get; }
        
        void SetMinHealth(float minHealth);
    }
}