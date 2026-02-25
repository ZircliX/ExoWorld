using System;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Core.Database;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.ShockGadget
{
    [CreateAssetMenu(menuName =  "OverBang/Gadgets/ShockGrenadeData")]
    public class ShockGrenadeData : GadgetData, IDatabaseAsset
    {
        [field: SerializeField] public ShockGrenadeEntity Prefab { get; private set; }
        [field: SerializeField] public DamageData DamageData { get; private set; }
        [field: SerializeField] public float ThrowForce { get; private set; }
        
        public string ID { get; private set; }
        
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(ID))
            {
                ID = Guid.NewGuid().ToString();
            }
        }
        
    }
}