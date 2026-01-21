using OverBang.ExoWorld.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
{
    public class ShockGrenadeData : GadgetData
    {
        [field: SerializeField] public ShockGrenade Prefab { get; private set; }
        [field: SerializeField] public DamageInfo DamageInfo { get; private set; }
        [field: SerializeField] public float ThrowForce { get; private set; }
        
    }
}