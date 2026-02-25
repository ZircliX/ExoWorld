using System;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Core.Database;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.LifePulseGadget
{
    [CreateAssetMenu(menuName = "OverBang/Gadgets/LifePulseData")]
    public class LifePulseData : GadgetData, IDatabaseAsset
    {
        [field: SerializeField] public LifePulseEntity Prefab { get; private set; }
        [field: SerializeField] public float HealthAmount { get; private set; }
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