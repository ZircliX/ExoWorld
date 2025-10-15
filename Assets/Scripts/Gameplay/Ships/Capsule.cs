using OverBang.GameName.Gameplay.Gameplay;
using OverBang.GameName.Gameplay.Gameplay.Listeners;
using OverBang.GameName.Quests.QuestData;
using OverBang.GameName.Quests.QuestEvents;
using OverBang.GameName.Quests.QuestHandlers;
using UnityEngine;
using ZTools.ObjectiveSystem.Core;
using ZTools.ObjectiveSystem.Core.Enum;
using ZTools.ObjectiveSystem.Core.Helpers;
using ZTools.ObjectiveSystem.Core.Interfaces;

namespace OverBang.GameName.Gameplay.Ships
{
    public class Capsule : GameplayListener
    {
        [SerializeField] private GameObject body;

        protected override void Initialize(GameplayPhase phase)
        {
            ObjectivesManager.OnObjectiveProgress += HandleObjectiveProgressChanged;
            body.SetActive(false);
        }

        protected override void Release(GameplayPhase phase)
        {
            ObjectivesManager.OnObjectiveProgress -= HandleObjectiveProgressChanged;
        }

        private void HandleObjectiveProgressChanged(IObjectiveHandler handler)
        {
            if (!handler.CastHandlerAndData(out ReachPointHandler reachPointHandler,
                    out ReachPointData reachPointData)) return;
            
            if (reachPointHandler.State != ObjectiveState.Disposed ||
                reachPointData.PointID != "TestFacilityEnter") return;
            
            body.SetActive(true);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && body.activeSelf)
            {
                current.CompletePhase(true);
            }
        }
    }
}