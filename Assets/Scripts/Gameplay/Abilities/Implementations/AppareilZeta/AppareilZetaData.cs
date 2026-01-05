using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [CreateAssetMenu(fileName = "AppareilZeta Ability Data", menuName = "OverBang/Abilities/AppareilZeta")]
    public class AppareilZetaData : AbilityData
    {
        public override IAbility CreateInstance(GameObject owner)
        {
            return new AppareilZetaAbility(this, owner);
        }
    }
}