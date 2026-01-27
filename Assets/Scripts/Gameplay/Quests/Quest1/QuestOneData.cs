using OverBang.ExoWorld.Gameplay.Enemies;
using UnityEngine;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    [CreateAssetMenu(fileName = "QuestOne", menuName = "OverBang/ObjectiveData/QuestOne", order = 0)]
    public class QuestOneData : ObjectiveData
    {
        [field: SerializeField] public float RepairTimeRequired { get; private set; } = 180f;
        [field: SerializeField] public int TotalRepairHealth { get; private set; } = 10;
        [field: SerializeField] public EnemySpawnScenario EnemySpawnScenario { get; private set; }
        
        public override IObjectiveHandler GetHandler()
        {
            return new QuestOneHandler();
        }
    }
}