using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Core.Damage;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.ParalixGadget
{
    [CreateAssetMenu(menuName = "OverBang/Gadgets/ParalixGrenadeData")]
    public class ParalixGrenadeData : GadgetData
    {
        [field: SerializeField] public ParalixGrenadeEntity Prefab { get; private set; }
        [field: SerializeField] public DamageData DamageData { get; private set; }
        [field: SerializeField] public float ThrowForce { get; private set; }
        [field: SerializeField] public float StunDuration { get; private set; }
        [field: SerializeField] public float SlowPercentage { get; private set; } = 1f;
    }
}