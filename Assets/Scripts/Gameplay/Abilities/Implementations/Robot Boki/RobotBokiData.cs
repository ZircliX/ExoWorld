using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [CreateAssetMenu(fileName = "RobotBoki Ability Data", menuName = "OverBang/Abilities/Robot Boki")]
    public class RobotBokiData : AbilityData
    {
        public override IAbility CreateInstance(GameObject owner)
        {
            return new RobotBokiAbility(this, owner);
        }
    }
}