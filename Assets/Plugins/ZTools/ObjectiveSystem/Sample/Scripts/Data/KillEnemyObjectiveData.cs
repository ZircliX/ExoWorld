using UnityEngine;
using ZTools.ObjectiveSystem.Core.Data;
using ZTools.ObjectiveSystem.Core.Interfaces;

namespace ZTools.ObjectiveSystem.Sample.Data
{
    /// <summary>
    /// Concrete ObjectiveData for a "Kill X Enemies" objective.
    /// Defines data specific to this type of objective.
    /// </summary>
    [CreateAssetMenu(fileName = "KillEnemyObjectiveData", menuName = "ZTools/ObjectiveSystem/ObjectiveData/Kill Enemy")]
    public class KillEnemyObjectiveData : ObjectiveData
    {
        /// <summary>
        /// Target tag for the enemy to kill
        /// </summary>
        [field: Header("Specific KillEnemy Objective Datas")]
        [field: SerializeField] public string EnemyTag { get; protected set; }
        
        /// <summary>
        /// The number of times the player will have to perform the task
        /// (e.g., number of enemies to kill, items to collect) to complete this objective.
        /// </summary>
        [field: SerializeField] public int ObjectiveCount { get; protected set; }

        public override IObjectiveHandler GetHandler()
        {
            return new KillEnemyHandler();
        }
    }
}