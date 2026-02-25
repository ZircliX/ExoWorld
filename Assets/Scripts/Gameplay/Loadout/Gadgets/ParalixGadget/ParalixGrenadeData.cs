using System;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Core.Database;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.ParalixGadget
{
    [CreateAssetMenu(menuName = "OverBang/Gadgets/ParalixGrenadeData")]
    public class ParalixGrenadeData : GadgetData, IDatabaseAsset
    {
        [field: SerializeField] public ParalixGrenadeEntity Prefab { get; private set; }
        [field: SerializeField] public DamageData DamageData { get; private set; }
        [field: SerializeField] public float ThrowForce { get; private set; }
        [field: SerializeField] public float StunDuration { get; private set; }
        [field: SerializeField] public float SlowPercentage { get; private set; } = 1f;
        
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