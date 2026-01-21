namespace OverBang.ExoWorld.Gameplay
{
    public interface IEffectMetricResolver
    {
        float Resolve(IEffectReceiver receiver, EffectData effectData);
    }
}