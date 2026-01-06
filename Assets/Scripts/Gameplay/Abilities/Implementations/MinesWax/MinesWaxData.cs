using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [CreateAssetMenu(menuName = "OverBang/Abilities/Mines Wax", fileName = "MinesWax Ability Data")]
    public class MinesWaxData : AbilityData
    {
        [field: Space]
        [field: SerializeField] public float ThrowForce { get; private set; }
        [field: SerializeField] public MineWax MineWaxPrefab { get; private set; }
        [field: SerializeField] public float DetectionRadius { get; private set; }
        [field: SerializeField] public float ExplosionRadius { get; private set; }
        [field: SerializeField] public DamageInfo DamageInfo { get; private set; }
    }
}