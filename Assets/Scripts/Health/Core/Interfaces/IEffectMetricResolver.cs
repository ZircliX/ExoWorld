namespace Health.Core
{
    public interface IEffectMetricResolver
    {
        float Resolve(IEffectReceiver receiver, EffectData effectData);
    }
}