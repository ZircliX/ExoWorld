using OverBang.GameName.Quests.QuestHandlers;
using UnityEngine;
using ZTools.ObjectiveSystem.Core.Data;
using ZTools.ObjectiveSystem.Core.Interfaces;

namespace OverBang.GameName.Quests.QuestData
{
    [CreateAssetMenu(fileName = "RepairData", menuName = "OverBang/ObjectiveData/RepairData", order = 0)]
    public class RepairData : ObjectiveData
    {
        public override IObjectiveHandler GetHandler()
        {
            return new RepairHandler();
        }
    }
}