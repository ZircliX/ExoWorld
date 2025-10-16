using OverBang.GameName.Quests.QuestData;
using OverBang.GameName.Quests.QuestEvents;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.GameName.Quests.QuestHandlers
{
    public class RepairHandler : ObjectiveHandler<RepairData, RepairEvent>
    {
        protected override ObjectiveProgression CalculateProgression(RepairData objectiveData, RepairEvent gameEvent)
        {
            return new ObjectiveProgression(gameEvent.RepairAmount, gameEvent.MaxRepairAmount);
        }

        protected override void ObjectiveCompleted()
        {
        }
    }
}