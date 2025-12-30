using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Dash
{
    [CreateAssetMenu(fileName = "DashData", menuName = "OverBang/Abilities/DashData")]
    public class DashData : AbilityData
    {
        [field: SerializeField] public DamageInfo Damages { get; private set; }
        
        public override IAbility CreateInstance(GameObject owner)
        {
            return new Dash(this, owner);
        }
    }
}