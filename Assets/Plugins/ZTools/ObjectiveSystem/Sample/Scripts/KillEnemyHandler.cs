using ZTools.ObjectiveSystem.Sample.Events;
using ZTools.ObjectiveSystem.Sample.Data;
using ZTools.ObjectiveSystem.Core;
using ZTools.RewardSystem.Core;

namespace ZTools.ObjectiveSystem.Sample
{
    /// <summary>
    /// Concrete runtime implementation of an objective with the typed ObjectiveData and GameCommand
    /// processed by this handler.
    /// </summary>
    public class KillEnemyHandler : ObjectiveHandler<KillEnemyObjectiveData, KillEnemyEvent>
    {
        private int totalKills;

        protected override ObjectiveProgression CalculateProgression(KillEnemyObjectiveData objectiveData, KillEnemyEvent gameEvent)
        {
            // Check if the command matches the objective's enemy tag
            // if true, the objective progresses by the kill count
            if (gameEvent.EnemyTag == objectiveData.EnemyTag)
            {
                totalKills += gameEvent.KillCount;
                return new ObjectiveProgression(totalKills, objectiveData.ObjectiveCount);
            }
            
            return default; // No progress made by this command
        }

        protected override void ObjectiveCompleted()
        {
            RewardManager.ProcessRewards(ObjectiveData.Rewards);
        }
    }
}