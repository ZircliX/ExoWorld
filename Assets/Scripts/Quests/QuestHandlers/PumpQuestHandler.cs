using OverBang.GameName.Quests.QuestData;
using OverBang.GameName.Quests.QuestEvents;
using ZTools.ObjectiveSystem.Core;
using ZTools.ObjectiveSystem.Core.ZTools.ObjectiveSystem.Core;

namespace OverBang.GameName.Quests.QuestHandlers
{
    public class PumpQuestHandler : ObjectiveHandler<PumpQuestData, PumpEvent>
    {
        protected override ObjectiveProgression CalculateProgression(PumpQuestData objectiveData, PumpEvent gameEvent)
        {
            return new ObjectiveProgression(gameEvent.RepairAmount, gameEvent.MaxRepairAmount);
        }

        protected override void ObjectiveCompleted()
        {
        }
    }
}