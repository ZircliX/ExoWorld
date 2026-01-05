using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [System.Serializable]
    public class AssaultDashStrategyData : IAbilityStrategyData, IDashAbilityStrategyData
    {
        [field: SerializeField] public DamageInfo FlameDamage { get; private set; }
    }
    
    [System.Serializable]
    public class ElectricDashStrategyData : IAbilityStrategyData, IDashAbilityStrategyData
    {
        [field: SerializeField] public DamageInfo BaseSpectralDamage { get; private set; }
    }
    
    [System.Serializable]
    public class SpectralDashStrategyData : IAbilityStrategyData, IDashAbilityStrategyData
    {
        [field: SerializeField] public int DashCount { get; private set; }
    }
    
    public interface IDashAbilityStrategyData { }
}