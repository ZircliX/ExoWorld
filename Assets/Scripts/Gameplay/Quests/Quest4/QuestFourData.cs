using UnityEngine;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    [CreateAssetMenu(fileName = "QuestFour", menuName = "OverBang/ObjectiveData/QuestFour", order = 2)]
    public class QuestFourData : ObjectiveData
    {
        public override IObjectiveHandler GetHandler()
        {
            return new QuestFourHandler();
        }
    }
}