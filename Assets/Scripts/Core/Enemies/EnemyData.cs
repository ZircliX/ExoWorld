using System;
using System.Collections.Generic;
using OverBang.GameName.Core;
using OverBang.Pooling;
using OverBang.Pooling.Dependencies;
using OverBang.Pooling.Resource;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [CreateAssetMenu(menuName = "OverBang/Enemy/EnemyData")]
    public class EnemyData : ScriptableObject, IPoolDependencyProvider, IDatabaseAsset
    {
        [field: SerializeField] public string EnemyName { get; private set; }
        [field: SerializeField] public EnemyType EnemyType { get; private set; }
        [field: SerializeField] public DamageInfo DamageInfo { get; private set; }
        [field: SerializeField] public float BaseHealth { get; private set; }
        
        [field: SerializeField] 
        public GameObject ModelPrefab { get; private set; }
        [field: SerializeField] 
        public NetworkObject EnemyPrefab { get; private set; }
        
        [field: SerializeField, Space] 
        public SimplePoolConfig[] Dependencies { get; private set; }

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
                if(Dependencies[i])
                    poolConfigs.Add(Dependencies[i]);
            }
        }
        
        
    }
}