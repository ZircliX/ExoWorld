using System;

namespace OverBang.ExoWorld.Gameplay.Targeting
{
    public interface IHealth
    {
        event Action<float, float, float> OnHealthChanged;
        
        float MinHealth { get; }
        float Health { get; }
        float MaxHealth { get; }
        
        void SetMinHealth(float minHealth);
    }
}