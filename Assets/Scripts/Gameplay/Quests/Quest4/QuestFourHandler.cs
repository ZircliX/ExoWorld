using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class QuestFourHandler : ObjectiveHandler<QuestFourData, QuestFourEvent>
    {
        protected override ObjectiveProgression CalculateProgression(QuestFourData objectiveData, QuestFourEvent gameEvent)
        {
            return new ObjectiveProgression(1, 1);
        }

        protected override void ObjectiveCompleted()
        {
        }
    }
}