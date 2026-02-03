using ZTools.ObjectiveSystem.Core;
using Object = UnityEngine.Object;

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
            GazDispenser gaz = Object.FindFirstObjectByType<GazDispenser>();
            if (gaz != null)
            {
                gaz.SetActiveState(false);
            }
        }
    }
}