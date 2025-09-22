using OverBang.GameName.Quests.QuestEvents;
using UnityEngine;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.GameName.Gameplay.Hub
{
    public class HubStartButton : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ObjectivesManager.DispatchGameEvent(new HubStartEvent());
            }
        }
    }
}