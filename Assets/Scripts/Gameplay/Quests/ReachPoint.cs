using OverBang.GameName.Quests.QuestData;
using OverBang.GameName.Quests.QuestEvents;
using UnityEngine;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.GameName.Gameplay.Quests
{
    [RequireComponent(typeof(Collider))]
    public class ReachPoint : MonoBehaviour
    {
        [SerializeField] private ReachPointData data;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ReachPointEvent gameEvent = new ReachPointEvent(data.PointID);
                
                ObjectivesManager.DispatchGameEvent(gameEvent);
                gameObject.SetActive(false);
            }
        }
    }
}