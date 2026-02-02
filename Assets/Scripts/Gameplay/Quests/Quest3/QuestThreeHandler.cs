using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class QuestThreeHandler : ObjectiveHandler<QuestThreeData, QuestThreeEvent>
    {
        private int currentKilledSpore;
        
        protected override ObjectiveProgression CalculateProgression(QuestThreeData objectiveData, QuestThreeEvent gameEvent)
        {
            currentKilledSpore++;
            return new ObjectiveProgression(currentKilledSpore, objectiveData.TargetSporeToKill);
        }

        protected override void ObjectiveCompleted()
        {
            
        }
    }
}