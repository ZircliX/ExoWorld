using OverBang.GameName.Gameplay.Core.Interfaces;
using OverBang.GameName.Gameplay.Core.Structs;

namespace OverBang.GameName.Gameplay.Core
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