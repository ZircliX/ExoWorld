using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class QuestTwoHandler : ObjectiveHandler<QuestTwoData, QuestTwoEvent>
    {
        private int totalPieces;
        
        protected override ObjectiveProgression CalculateProgression(QuestTwoData objectiveData, QuestTwoEvent gameEvent)
        {
            totalPieces += gameEvent.pieces;
            return new ObjectiveProgression(totalPieces, objectiveData.TotalPieces);
        }

        protected override void ObjectiveCompleted()
        {
            
        }
    }
}