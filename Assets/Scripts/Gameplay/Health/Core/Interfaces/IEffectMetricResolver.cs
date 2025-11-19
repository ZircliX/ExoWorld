using OverBang.GameName.Gameplay.Core.Structs;

namespace OverBang.GameName.Gameplay.Core.Interfaces
{
    public interface IEffectMetricResolver
    {
        float Resolve(IEffectReceiver receiver, EffectData effectData);
    }
}