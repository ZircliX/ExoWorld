using UnityEngine;
using ZTools.RewardSystem.Core.Data;

namespace ZTools.ObjectiveSystem.Core.Data
{
    [CreateAssetMenu(fileName = "ObjectiveCollectionData", menuName = "ZTools/ObjectiveSystem/ObjectiveCollectionData", order = 1)]
    public class ObjectiveCollectionData : ScriptableObject
    {
        /// <summary>
        /// The name of this objective collection, used for identification or display purposes.
        /// </summary>
        [field: SerializeField] public string CollectionName { get; private set; }
        public string GetCollectionName() => CollectionName;

        /// <summary>
        /// An array of <see cref="ObjectiveData"/> assets that constitute the sequence of objectives
        /// within this collection. These objectives are typically processed in the order they appear in the array.
        /// </summary>
        [field: SerializeField] public ObjectiveData[] Objectives { get; private set; }
        public ObjectiveData[] GetObjectives() => Objectives;
        
        /// <summary>
        /// An array of <see cref="RewardData"/> assets that are processed when all objectives in this collection are completed.
        /// </summary>
        [field: SerializeField] public RewardData[] Rewards {get; private set; }
        public RewardData[] GetRewards() => Rewards;
    }
}