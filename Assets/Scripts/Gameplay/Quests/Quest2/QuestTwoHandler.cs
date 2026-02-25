using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class QuestTwoHandler : ObjectiveHandler<QuestTwoData, QuestTwoEvent>
    {
        protected override ObjectiveProgression CalculateProgression(QuestTwoData objectiveData, QuestTwoEvent gameEvent)
        {
            return new ObjectiveProgression(gameEvent.pieces, gameEvent.targetPieces);
        }

        protected override void ObjectiveCompleted()
        {
            
        }
    }
}