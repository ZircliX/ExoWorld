using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Core.Damage;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.FrostBiteGadget
{
    [CreateAssetMenu(menuName =  "OverBang/Gadgets/FrostBiteGrenadeData")]
    public class FrostBiteGrenadeData : GadgetData
    {
        [field: SerializeField] public FrostBiteGrenadeEntity Prefab { get; private set; }
        [field: SerializeField] public DamageData DamageData { get; private set; }
        [field: SerializeField] public float ThrowForce { get; private set; }
        [field: SerializeField] public float SlowDuration { get; private set; }
        [field: SerializeField] public float SlowPercentage { get; private set; }
    }
}