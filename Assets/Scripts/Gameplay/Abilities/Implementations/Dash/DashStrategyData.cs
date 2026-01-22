using OverBang.ExoWorld.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    [System.Serializable]
    public class DashStrategyData : IAbilityStrategyData, IDashAbilityStrategyData
    {
        [field: SerializeField] public DamageInfo FlameDamage { get; private set; }
    }
    
    [System.Serializable]
    public class ElectricDashStrategyData : IAbilityStrategyData, IDashAbilityStrategyData
    {
        [field: SerializeField] public DamageInfo ElectricDamage { get; private set; }
        [field: SerializeField] public float ExplosionRadius { get; private set; }
    }
    
    [System.Serializable]
    public class SpectralDashStrategyData : IAbilityStrategyData, IDashAbilityStrategyData
    {
        [field: SerializeField] public int DashCount { get; private set; }
    }
    
    public interface IDashAbilityStrategyData { }
}