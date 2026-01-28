using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.Damage;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    [System.Serializable]
    public class MinesWaxStrategyData : IAbilityStrategyData, IMinesWaxStrategyData
    {
        [field: SerializeField] public DamageData Damage { get; private set; }
    }
    
    [System.Serializable]
    public class MinesWaxCryoStrategyData : IAbilityStrategyData, IMinesWaxStrategyData
    {
        [field: SerializeField] public DamageData Damage { get; private set; }
        [field: SerializeField] public float SlowDuration { get; private set; }
        [field: SerializeField] public float SlowPercentage { get; private set; }
    }
    
    [System.Serializable]
    public class MinesWaxNovaStrategyData : IAbilityStrategyData, IMinesWaxStrategyData
    {
        [field: SerializeField] public DamageData Damage { get; private set; }
        [field: SerializeField] public float ExplosionInterval { get; private set; }
        [field: SerializeField] public int ExplosionCount { get; private set; }
    }
    
    public interface IMinesWaxStrategyData {}
}