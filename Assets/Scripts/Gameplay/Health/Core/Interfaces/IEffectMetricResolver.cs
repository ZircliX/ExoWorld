namespace OverBang.ExoWorld.Gameplay.Health
{
    public interface IEffectMetricResolver
    {
        float Resolve(IEffectReceiver receiver, EffectData effectData);
    }
}