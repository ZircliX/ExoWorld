using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class QuestOneHandler : ObjectiveHandler<QuestOneData, QuestOneEvent>
    {
        protected override ObjectiveProgression CalculateProgression(QuestOneData objectiveOneData, QuestOneEvent gameEvent)
        {
            return new ObjectiveProgression(gameEvent.RepairAmount, gameEvent.MaxRepairAmount);
        }

        protected override void ObjectiveCompleted()
        {
        }
    }
}