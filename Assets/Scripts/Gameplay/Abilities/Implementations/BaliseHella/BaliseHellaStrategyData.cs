using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [System.Serializable]
    public class BaliseHellaStrategyData : IAbilityStrategyData, IBaliseHellaAbilityStrategyData
    {
        [field: SerializeField] public float Radius { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float MinHealth { get; private set; }
    }
    
    [System.Serializable]
    public class BaliseSecondChanceStrategyData : IAbilityStrategyData, IBaliseHellaAbilityStrategyData
    {
        [field: SerializeField] public float Radius { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float MinHealth { get; private set; }
        [field: SerializeField] public float HealingPerSecond { get; private set; }
    }
    
    [System.Serializable]
    public class BaliseInvisibilityStrategyData : IAbilityStrategyData, IBaliseHellaAbilityStrategyData
    {
        [field: SerializeField] public float Radius { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
    }

    public interface IBaliseHellaAbilityStrategyData
    {
        float Radius { get; }
        float Duration { get; }
    }
}
