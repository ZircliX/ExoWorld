using OverBang.GameName.Quests.QuestHandlers;
using UnityEngine;
using ZTools.ObjectiveSystem.Core.Data;
using ZTools.ObjectiveSystem.Core.Interfaces;

namespace OverBang.GameName.Quests.QuestData
{
    [CreateAssetMenu(fileName = "ReachPointData", menuName = "OverBang/ObjectiveData/ReachPointData", order = 0)]
    public class ReachPointData : ObjectiveData
    {
        [field: SerializeField] public string PointID { get; protected set; }
        [field: SerializeField] public int ObjectiveCount { get; protected set; }
        
        public override IObjectiveHandler GetHandler()
        {
            return new ReachPointHandler();
        }
    }
}