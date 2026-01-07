using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [System.Serializable]
    public class MinesWaxStrategyData : IAbilityStrategyData, IMinesWaxStrategyData
    {
        [field: SerializeField] public DamageInfo Damage { get; private set; }
    }
    
    [System.Serializable]
    public class MinesWaxCryoStrategyData : IAbilityStrategyData, IMinesWaxStrategyData
    {
        [field: SerializeField] public DamageInfo Damage { get; private set; }
        [field: SerializeField] public float SlowDuration { get; private set; }
        [field: SerializeField] public float SlowPercentage { get; private set; }
    }
    
    [System.Serializable]
    public class MinesWaxNovaStrategyData : IAbilityStrategyData, IMinesWaxStrategyData
    {
        [field: SerializeField] public DamageInfo Damage { get; private set; }
        [field: SerializeField] public float ExplosionInterval { get; private set; }
        [field: SerializeField] public int ExplosionCount { get; private set; }
    }
    
    public interface IMinesWaxStrategyData {}
}