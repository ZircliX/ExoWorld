using OverBang.ExoWorld.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    [CreateAssetMenu(fileName = "Dash Ability Data", menuName = "OverBang/Abilities/DashData")]
    public class DashData : AbilityData
    {
        [field: Space]
        [field: SerializeField] public DamageInfo Damage { get; private set; }
        [field: SerializeField] public float CastDistanceThreshold { get; private set; } = 0.2f;
    }
}