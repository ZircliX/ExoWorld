using System;
using System.Collections.Generic;
using OverBang.ExoWorld.Core.Database;
using OverBang.ExoWorld.Core.Upgrade;
using OverBang.Pooling;
using OverBang.Pooling.Dependencies;
using OverBang.Pooling.Resource;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Characters
{
    [CreateAssetMenu(fileName = "New Agent Data", menuName = "OverBang/Agent Data", order = 0)]
    public class CharacterData : ScriptableObject, IPoolDependencyProvider, IDatabaseAsset
    {
        [field: SerializeField] 
        public string Name { get; private set; }
        
        [field: SerializeField] 
        public string Description { get; private set; }
        
        [field: SerializeField]
        public Sprite Sprite { get; private set; }
        
        [field: SerializeField]
        public Color Color { get; private set; }
        
        [field: SerializeField] 
        public CharacterClasses CharacterClass { get; private set; }
        
        [field: SerializeField] 
        public GameObject ModelPrefab { get; private set; }
        
        [field: SerializeField] 
        public UpgradeData[] UpgradeDatas { get; private set; }

        [field : SerializeField] 
        public CharacterBaseStats BaseStats { get; private set; }
        
        [field: SerializeField]
        public AbilityData PrimaryAbility { get; private set; }
        [field: SerializeField]
        public AbilityData SecondaryAbility { get; private set; }
        
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