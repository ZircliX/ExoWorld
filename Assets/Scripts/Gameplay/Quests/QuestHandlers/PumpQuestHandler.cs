using ZTools.ObjectiveSystem.Core;

namespace OverBang.GameName.Gameplay
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