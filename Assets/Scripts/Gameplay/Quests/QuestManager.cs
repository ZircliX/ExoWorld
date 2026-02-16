using UnityEngine;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class QuestManager
    {
        private readonly ObjectiveData[] quests;
        private ObjectiveData currentQuest;
        private int currentQuestIndex;
        
        public QuestManager()
        {
            quests = Resources.LoadAll<ObjectiveData>("Quests");
            ObjectivesManager.OnObjectiveProgress += OnObjectiveProgress;
        }

        private void OnObjectiveProgress(IObjectiveHandler handler)
        {
            if (handler.State is ObjectiveState.Completed or ObjectiveState.Disposed &&
                handler.ObjectiveData == currentQuest)
            {
                Debug.Log($"Quest {currentQuest.Name} completed!");
                currentQuest = null;
            }
        }

        public void RequestQuestQueue()
        {
            if (currentQuest == null)
            {
                Debug.Log("Requesting quest queue");
                QueueNextQuest();
            }
        }

        public void QueueNextQuest()
        {
            if (currentQuestIndex >= quests.Length) return;
            
            currentQuest = quests[currentQuestIndex];
            currentQuest.AddObjective();
            //Debug.Log($"Queuing quest {currentQuest.Name}");
            
            currentQuestIndex++;
        }
    }
}