using System.Collections.Generic;
using OverBang.Pooling;
using OverBang.Pooling.Dependencies;
using OverBang.Pooling.Resource;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace OverBang.GameName.Core.Characters
{
    [CreateAssetMenu(fileName = "New Agent Data", menuName = "OverBang/Agent Data", order = 0)]
    public class CharacterData : ScriptableObject, IPoolDependencyProvider
    {
        [field: SerializeField] 
        public string AgentName { get; private set; }
        [field: SerializeField]
        public Sprite AgentSprite { get; private set; }
        [field: SerializeField] 
        public CharacterClasses CharacterClass { get; private set; }
        [field: SerializeField] 
        public AssetReferenceT<GameObject> CharacterPrefabRef { get; private set; }
        [field: SerializeField] 
        public SimplePoolConfig[] Dependencies { get; private set; }
        
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