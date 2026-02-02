using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Core.Damage;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.C4Gadget
{
    [CreateAssetMenu(menuName =  "OverBang/Gadgets/C4Data")]
    public class C4Data : GadgetData
    {
        [field: SerializeField] public C4Entity Prefab { get; private set; }
        [field: SerializeField] public DamageData DamageData { get; private set; }
        [field: SerializeField] public float ThrowForce { get; private set; }
    }
}