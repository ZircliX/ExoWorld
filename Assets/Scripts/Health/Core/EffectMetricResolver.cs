namespace Health.Core
{
    public struct PercentageMetricResolver  : IEffectMetricResolver
    {
        public float ResolveAmount(IEffectReceiver receiver, EffectData effectData)
        {
            return (effectData.PercentageAmount / 100f) * receiver.MaxValue;
        }
    }
    
    public struct PointsMetricResolver : IEffectMetricResolver
    {
        public float ResolveAmount(IEffectReceiver receiver, EffectData effectData)
        {
            return effectData.Amount;
        }
    }
}