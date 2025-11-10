using OverBang.GameName.Quests.QuestData;
using OverBang.GameName.Quests.QuestEvents;
using ZTools.ObjectiveSystem.Core;
using ZTools.ObjectiveSystem.Core.ZTools.ObjectiveSystem.Core;
using ZTools.ObjectiveSystem.Core.ZTools.ObjectiveSystem.Core.Structs;
using ZTools.RewardSystem.Core;
using ZTools.RewardSystem.Core.ZTools.RewardSystem.Core;

namespace OverBang.GameName.Quests.QuestHandlers
{
    public class ReachPointHandler : ObjectiveHandler<ReachPointData, ReachPointEvent>
    {
        protected override ObjectiveProgression CalculateProgression(ReachPointData objectiveData, ReachPointEvent gameEvent)
        {
            if (gameEvent.PointID == objectiveData.PointID)
            {
                return new ObjectiveProgression(1, objectiveData.ObjectiveCount);
            }
            
            return default; // No progress made by this command
        }

        protected override void ObjectiveCompleted()
        {
            RewardManager.ProcessRewards(DataTyped.Rewards);
        }
    }
}