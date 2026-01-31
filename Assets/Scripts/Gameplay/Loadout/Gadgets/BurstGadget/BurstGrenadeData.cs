using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Core.Damage;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.BurstGadget
{
    [CreateAssetMenu(menuName =  "OverBang/Gadgets/BurstGrenadeData")]
    public class BurstGrenadeData : GadgetData
    {
        [field: SerializeField] public BurstGrenadeEntity Prefab { get; private set; }
        [field: SerializeField] public DamageData DamageData { get; private set; }
        [field: SerializeField] public float ThrowForce { get; private set; }
    }
}