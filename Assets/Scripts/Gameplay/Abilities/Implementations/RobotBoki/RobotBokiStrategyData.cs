using OverBang.ExoWorld.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    [System.Serializable]
    public class RobotBokiStrategyData : IAbilityStrategyData, IRobotBokiAbilityStrategyData
    {
        [field: SerializeField] public DamageInfo Damage { get; private set; }
        [field: SerializeField] public float ExplosionRadius { get; private set; }
        [field: SerializeField] public float DetectionRadius { get; private set; }
    }
    
    [System.Serializable]
    public class RobotBokiLeurreStrategyData : IAbilityStrategyData, IRobotBokiAbilityStrategyData
    {
        [field: SerializeField] public float ExplosionRadius { get; private set; }
        [field: SerializeField] public float DetectionRadius { get; private set; }
    }
    
    [System.Serializable]
    public class RobotBokiImpulsionStrategyData : IAbilityStrategyData, IRobotBokiAbilityStrategyData
    {
        [field: SerializeField] public DamageInfo Damage { get; private set; }
        [field: SerializeField] public float ExplosionRadius { get; private set; }
        [field: SerializeField] public float DetectionRadius { get; private set; }
        [field: SerializeField] public float FreezeDuration { get; private set; }
    }

    public interface IRobotBokiAbilityStrategyData
    {
        float ExplosionRadius { get; }
        float DetectionRadius { get; }
    }
}