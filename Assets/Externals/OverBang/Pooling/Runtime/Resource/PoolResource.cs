using System.Collections.Generic;
using OverBang.Pooling.Dependencies;
using UnityEngine;

namespace OverBang.Pooling.Resource
{
    [CreateAssetMenu(fileName = "New PoolResource", menuName = "OverBang/Pooling/Resources")]
    public class PoolResource : ScriptableObject, IPoolDependencyProvider
    {
        [field: SerializeField]
        public PoolEmptyBehavior PoolEmptyBehavior { get; private set; }
        
        [field: SerializeReference] 
        public IPoolAsset Asset { get; private set; }
        
        [field: SerializeField]
        public SimplePoolConfig[] Dependencies { get; private set; }

        void IPoolDependencyProvider.FillDependencies(List<IPoolConfig> poolConfigs)
        {
            for (int i = 0; i < Dependencies.Length; i++)
                poolConfigs.Add(Dependencies[i]);
        }
    }
}