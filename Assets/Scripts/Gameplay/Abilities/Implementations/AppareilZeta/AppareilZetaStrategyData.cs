using OverBang.ExoWorld.Core.Characters;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    [System.Serializable]
    public class AppareilZetaStrategyData : IAbilityStrategyData, IAppareilZetaAbilityStrategyData
    {
        [field: SerializeField] public float ExplosionRadius { get; private set; }
        [field: SerializeField] public float ExplosionDamage { get; private set; }
        [field: SerializeField] public float ExplosionDamageTick { get; private set; }
        [field: SerializeField] public float GivenBonus { get; private set; }
        [field: SerializeField] public float MoveSpeedBonus { get; private set; }
    }
    
    [System.Serializable]
    public class AppareilZetaPersistantStrategyData : IAbilityStrategyData, IAppareilZetaAbilityStrategyData
    {
        [field: SerializeField] public float ExplosionRadius { get; private set; }
        [field: SerializeField] public float ExplosionDamage { get; private set; }
        [field: SerializeField] public float ExplosionDamageTick { get; private set; }
        [field: SerializeField] public float GivenBonus { get; private set; }
        [field: SerializeField] public float MoveSpeedBonus { get; private set; }
    }
    
    [System.Serializable]
    public class AppareilZetaCompressedStrategyData : IAbilityStrategyData, IAppareilZetaAbilityStrategyData
    {
        [field: SerializeField] public float ExplosionRadius { get; private set; }
        [field: SerializeField] public float ExplosionDamage { get; private set; }
        [field: SerializeField] public float ExplosionDamageTick { get; private set; }
        [field: SerializeField] public float GivenBonus { get; private set; }
        [field: SerializeField] public float MoveSpeedBonus { get; private set; }
    }

    public interface IAppareilZetaAbilityStrategyData
    {
        float ExplosionRadius { get; }
        float ExplosionDamage { get; }
        float ExplosionDamageTick { get; }
        float GivenBonus { get; }
        float MoveSpeedBonus { get; }
    }
}