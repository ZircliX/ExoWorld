namespace Health.Core
{
    public interface IEffectMetricResolver
    {
        float ResolveAmount(IEffectReceiver receiver, EffectData effectData);
    }
}