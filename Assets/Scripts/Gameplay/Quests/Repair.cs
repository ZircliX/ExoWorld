using OverBang.GameName.Quests.QuestEvents;
using UnityEngine;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.GameName.Gameplay.Quests
{
    [RequireComponent(typeof(Collider))]
    public class Repair : MonoBehaviour
    {
        [SerializeField] private GameObject ui;
        private float currentRepairAmount;
        private float totalRepairAmount = 10f;
        
        private void OnTriggerStay(Collider other)
        {
            currentRepairAmount += Time.deltaTime;
            RepairEvent evt = new RepairEvent(currentRepairAmount, totalRepairAmount);
            ObjectivesManager.DispatchGameEvent(evt);

            if (currentRepairAmount >= totalRepairAmount)
            {
                transform.parent.gameObject.SetActive(false);
                ui.SetActive(true);
                Destroy(ui, 2.5f);
            }
        }
    }
}