using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [CreateAssetMenu(fileName = "PistoletHella Ability Data", menuName = "OverBang/Abilities/PistoletHella")]
    public class PistoletHellaData: AbilityData
    {
        public override IAbility CreateInstance(GameObject owner)
        {
            return new PistoletHellaAbility(this, owner);
        }
    }
}