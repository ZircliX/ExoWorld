using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [CreateAssetMenu(menuName = "OverBang/Abilities/GraineDeChanui", fileName = "GraineDeChanui Ability Data")]
    public class GraineDeChanuiData : AbilityData
    {
        public override IAbility CreateInstance(GameObject owner)
        {
            return new GraineDeChanuiAbility(this, owner);
        }
    }
}