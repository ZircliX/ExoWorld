using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [System.Serializable]
    public class DfoStrategyData : IAbilityStrategyData
    {
        //Only changing parameters for specific augment
        [field: SerializeField] public float ActivationTime { get; private set; }
        [field: SerializeField] public int MissileCount { get; private set; }
        [field: SerializeField] public float DiameterSpawn { get; private set; }
        [field: SerializeField] public DamageInfo Damage { get; private set; }
    }
}