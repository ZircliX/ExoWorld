using System;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Core.Database;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.FrostBiteGadget
{
    [CreateAssetMenu(menuName =  "OverBang/Gadgets/FrostBiteGrenadeData")]
    public class FrostBiteGrenadeData : GadgetData, IDatabaseAsset
    {
        [field: SerializeField] public NetworkObject Prefab { get; private set; }
        [field: SerializeField] public DamageData DamageData { get; private set; }
        [field: SerializeField] public float ThrowForce { get; private set; }
        [field: SerializeField] public float SlowDuration { get; private set; }
        [field: SerializeField] public float SlowPercentage { get; private set; }
        [field : SerializeField, ReadOnly] public string ID { get; private set; }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(ID))
            {
                ID = Guid.NewGuid().ToString();
            }
        }
    }
}