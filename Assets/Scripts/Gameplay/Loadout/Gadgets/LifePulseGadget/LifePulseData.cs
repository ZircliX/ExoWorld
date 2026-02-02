using OverBang.ExoWorld.Core.Abilities.Gadgets;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.LifePulseGadget
{
    [CreateAssetMenu(menuName = "OverBang/Gadgets/LifePulseData")]
    public class LifePulseData : GadgetData
    {
        [field: SerializeField] public LifePulseEntity Prefab { get; private set; }
        [field: SerializeField] public float ThrowForce { get; private set; }
    }
}