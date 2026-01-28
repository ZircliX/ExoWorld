using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.Damage;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    [System.Serializable]
    public class DfoStrategyData : IAbilityStrategyData
    {
        [field: SerializeField] public float ActivationTime { get; private set; }
        [field: SerializeField] public int MissileCount { get; private set; }
        [field: SerializeField] public float DiameterSpawn { get; private set; }
        [field: SerializeField] public DamageData Damage { get; private set; }
    }
}