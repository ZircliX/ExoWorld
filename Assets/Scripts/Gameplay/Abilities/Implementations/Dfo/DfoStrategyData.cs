using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [System.Serializable]
    public class DfoStrategyData : IAbilityStrategyData
    {
        [field: SerializeField] public float ActivationTime { get; private set; }
        [field: SerializeField] public int MissileCount { get; private set; }
        [field: SerializeField] public float DiameterSpawn { get; private set; }
        [field: SerializeField] public DamageInfo Damage { get; private set; }
    }
}