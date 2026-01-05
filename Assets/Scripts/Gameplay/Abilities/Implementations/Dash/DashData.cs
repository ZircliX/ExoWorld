using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [CreateAssetMenu(fileName = "Dash Ability Data", menuName = "OverBang/Abilities/DashData")]
    public class DashData : AbilityData
    {
        [field: SerializeField] public DamageInfo Damage { get; private set; }
    }
}