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
            if (handler.State is ObjectiveState.Completed  or ObjectiveState.Disposed &&
                handler.ObjectiveData == currentQuest)
            {
                //Debug.LogError($"Quest {currentQuest.Name} completed!");
                currentQuest = null;
            }
        }

        public void RequestQuestQueue(int skip = 0)
        {
            if (currentQuest == null)
            {
                //Debug.LogError("Requesting quest queue");
                QueueNextQuest(skip);
            }
            else
            {
                Debug.Log(currentQuest.Name);
            }
        }

        private void QueueNextQuest(int skip = 0)
        {
            if (currentQuestIndex >= quests.Length) return;
            
            int nextIndex = currentQuestIndex + skip;
            
            currentQuest = quests[nextIndex];
            currentQuest.AddObjective();
            Debug.Log($"Queuing quest {currentQuest.Name}");
            
            currentQuestIndex++;
        }
    }
}