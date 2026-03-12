    using System;
    using DamageNumbersPro;
    using OverBang.ExoWorld.Core.Abilities.Gadgets;
    using OverBang.ExoWorld.Core.Damage;
    using OverBang.ExoWorld.Core.Database;
    using Sirenix.OdinInspector;
    using Unity.Netcode;
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
            [field: SerializeField] public DamageNumberMesh DamagePrefab { get; private set; }
            [field : SerializeField, ReadOnly] public string ID { get; private set; }

            private void OnValidate()
            {
                if (string.IsNullOrEmpty(ID))
                {
                    ID = Guid.NewGuid().ToString();
                    Debug.LogWarning($"[OnValidate] Regenerating ID for {name}..." +
                                     $"{ID}");
                }
            }
            
            [Button]
            private void GenerateID()
            {
                ID = Guid.NewGuid().ToString();
                Debug.LogWarning($"[OnGenerateID] ID  manually generated for {name}..." +
                                 $"{ID}");
            }
            
        }
    }