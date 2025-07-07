namespace Health.Core
{
    public interface IEffectContext
    {
        void ApplyEffectTick(IEffectCommand sender, float effectValue);
        void ApplyEffect(EffectData effectData);
        void SetValue(IEffectCommand sender, float value);
        
        float CurrentValue { get; }
        float MaxValue { get; }
    }
}