namespace OverBang.GameName.Health
{
    public interface IEffectMetricResolver
    {
        float Resolve(IEffectReceiver receiver, EffectData effectData);
    }
}