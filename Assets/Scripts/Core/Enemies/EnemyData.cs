using System;
using System.Collections.Generic;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Core.Database;
using OverBang.ExoWorld.Core.Inventory;
using OverBang.Pooling;
using OverBang.Pooling.Dependencies;
using OverBang.Pooling.Resource;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Enemies
{
    [CreateAssetMenu(menuName = "OverBang/Enemy/EnemyData")]
    public class EnemyData : ScriptableObject, IPoolDependencyProvider, IDatabaseAsset
    {
        [field: SerializeField] public string EnemyName { get; private set; }
        [field: SerializeField] public EnemyType EnemyType { get; private set; }
        [field: SerializeField] public DamageData DamageData { get; private set; }
        [field: SerializeField] public float BaseHealth { get; private set; }
        [field: SerializeField] public float BaseSpeed { get; private set; }
        [field: SerializeField] public float RagdollDuration { get; private set; }
        [field: SerializeField] public float PatrolRadius { get; private set; } = 10f;
        [field: SerializeField] public float TriggerDetectionRadius { get; private set; }
        [field: SerializeField] public float AttackDetectionRadius { get; private set; }
        
        [field: SerializeField]
        public GameObject ModelPrefab { get; private set; }
        [field: SerializeField]
        public NetworkObject EnemyPrefab { get; private set; }
        [field: SerializeField]
        public LootTable LootTable { get; private set; }
        
        [field: SerializeField, Space]
        public SimplePoolConfigData[] Dependencies { get; private set; }

        [field: SerializeField, ReadOnly]
        public string ID { get; private set; }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(ID))
            {
                ID = Guid.NewGuid().ToString();
            }
        }

        void IPoolDependencyProvider.FillDependencies(List<IPoolConfig> poolConfigs)
        {
            if (Dependencies == null || Dependencies.Length == 0)
                return;
            
            for (int i = 0; i < Dependencies.Length; i++)
            {
                SimplePoolConfigData simplePoolConfigData = Dependencies[i];
                if (simplePoolConfigData == null)
                    continue;
                
                for (int j = 0; j < simplePoolConfigData.Config.Length; j++)
                {
                    SimplePoolConfig simplePoolConfig = simplePoolConfigData.Config[j];
                    if(simplePoolConfig)
                        poolConfigs.Add(simplePoolConfig);
                }
            }
        }
    }
}