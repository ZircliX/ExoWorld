namespace OverBang.GameName.Gameplay.Health
{
    public struct PercentageMetricResolver  : IEffectMetricResolver
    {
        public float Resolve(IEffectReceiver receiver, EffectData effectData)
        {
            return (effectData.PercentageAmount / 100f) * receiver.MaxValue;
        }
    }
    
    public struct PointsMetricResolver : IEffectMetricResolver
    {
        public float Resolve(IEffectReceiver receiver, EffectData effectData)
        {
            return effectData.Amount;
        }
    }
}