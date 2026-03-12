namespace OverBang.ExoWorld.Gameplay.Targeting
{
    public interface IHealth
    {
        event HealthChanged OnHealthChanged;
        public delegate void HealthChanged(float previous, float current, float max);
        
        float MinHealth { get; }
        float Health { get; }
        float MaxHealth { get; }
        
        void SetMinHealth(float minHealth);
    }
}