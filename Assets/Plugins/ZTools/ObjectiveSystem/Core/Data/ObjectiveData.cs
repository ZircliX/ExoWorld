using UnityEngine;
using ZTools.Logger.Core.Interfaces;
using ZTools.ObjectiveSystem.Core.Interfaces;
using ZTools.RewardSystem.Core.Data;
using ZTools.RewardSystem.Core.Interfaces;

namespace ZTools.ObjectiveSystem.Core.Data
{
    /// <summary>
    /// Base abstract class for all objective data ScriptableObjects.
    /// Contains common objective definition data and defines the completion strategy.
    /// </summary>
    public abstract class ObjectiveData : ScriptableObject, ILogSource
    {
        /// <summary>
        /// The name of the objective, displayed to the player.
        /// </summary>
        [field: Header("Objective Definition")]
        [field: SerializeField] public string ObjectiveName { get; protected set; }
        public string Name => name;
        public string GetObjectiveName() => ObjectiveName;

        /// <summary>
        /// A detailed description of the objective, explaining the task to the player.
        /// </summary>
        [field: SerializeField, TextArea] public string ObjectiveDescription { get; protected set; }
        public string GetObjectiveDescription() => ObjectiveDescription;

        /// <summary>
        /// An array of <see cref="RewardData"/> assets that are granted when the objective is completed.
        /// </summary>
        [field: SerializeField] public RewardData[] Rewards {get; private set; }
        public RewardData[] GetRewards() => Rewards;

        /// <summary>
        /// Abstract method to create and return a concrete runtime handler for this objective.
        /// </summary>
        /// <returns>An instance of <see cref="IObjectiveHandler"/> that will manage the objective's runtime behavior.</returns>
        public abstract IObjectiveHandler GetHandler();
    }
}