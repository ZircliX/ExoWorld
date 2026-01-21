using OverBang.ExoWorld.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
{
    [CreateAssetMenu(fileName = "RobotBoki Ability Data", menuName = "OverBang/Abilities/Robot Boki")]
    public class RobotBokiData : AbilityData
    {
        [field: Space]
        [field: SerializeField] public float Speed { get; private set; }
    }
}