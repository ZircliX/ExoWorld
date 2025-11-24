using OverBang.GameName.Gameplay.QuestHandlers;
using UnityEngine;
using ZTools.ObjectiveSystem.Core.ZTools.ObjectiveSystem.Core;

namespace OverBang.GameName.Gameplay.QuestData
{
    [CreateAssetMenu(fileName = "RepairData", menuName = "OverBang/ObjectiveData/RepairData", order = 0)]
    public class PumpQuestData : ObjectiveData
    {
        [field: SerializeField] public float RepairTimeRequired { get; private set; } = 180f;
        [field: SerializeField] public int TotalRepairHealth { get; private set; } = 10;
        [field: SerializeField] public EnemySpawnScenario EnemySpawnScenario { get; private set; }
        
        public override IObjectiveHandler GetHandler()
        {
            return new PumpQuestHandler();
        }
    }
}