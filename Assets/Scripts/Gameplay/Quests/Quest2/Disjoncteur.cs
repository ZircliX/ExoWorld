using OverBang.ExoWorld.Core.Interactions;
using OverBang.ExoWorld.Gameplay.Abilities;
using Unity.Netcode;
using UnityEngine;
using UnityUtils;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class Disjoncteur : NetworkBehaviour, IInteractable
    {
        [SerializeField] private QuestTwoData questData;
        [SerializeField] private DetectionArea detectionArea;

        private int fusiblesInserted = 0;
        private bool isHoldingFusible = false;
        private QuestTwoHandler questTwoHandler;
        
        public string InteractionText => CanInteract && isHoldingFusible ? questData.InteractionText : questData.InteractionTextEmpty;
        public int Priority => (int)TargetPriority.High;
        public bool CanInteract { get; private set; } = true;
        private void SetCanInteractTrue() => CanInteract = true;
        public InteractionType SupportedInteractions => InteractionType.Interact;

        Vector3 IInteractable.UIPosition => transform.position.Add(y: 1f);

        public void Interact(PlayerInteraction playerInteraction)
        {
            questTwoHandler ??= questData.GetHandlerByData<QuestTwoHandler>();
            if (questTwoHandler is {StepIndex: < 2})
                questTwoHandler.SetStepIndex(2);
            
            AddFusible(playerInteraction);

            CanInteract = false;
            Invoke(nameof(SetCanInteractTrue), 1f);
        }

        public void OnPlayerEnter(PlayerInteraction playerInteraction)
        {
            isHoldingFusible = playerInteraction.GetHoldingItemType<IFusible>();
        }

        private void OnEnable()
        {
            detectionArea.OnEnter += OnEnter;
            detectionArea.OnExit += OnExit;
        }

        private void OnDisable()
        {
            detectionArea.OnEnter -= OnEnter;
            detectionArea.OnExit -= OnExit;
        }

        private void Awake()
        {
            detectionArea.GetCollider<SphereCollider>().radius = questData.InteractionRange;
            detectionArea.SetRequireInterface<IFusible>();
        }

        private void AddFusible(PlayerInteraction playerInteraction)
        {
            if (!playerInteraction.IsHoldingItem())
                return;

            if (playerInteraction.GetHoldingItemType<IFusible>())
            {
                isHoldingFusible = false;
                fusiblesInserted++;
                playerInteraction.DropItem();

                QuestTwoEvent evt = new QuestTwoEvent(1);
                ObjectivesManager.DispatchGameEvent(evt);

                if (fusiblesInserted >= questData.TotalPieces)
                {
                    OnAllFusiblesInserted();
                }
            }
        }

        private void OnEnter(Collider col, object target)
        {
            if (target is IFusible fusible)
            {
                CanInteract = true;
            }
        }

        private void OnExit(Collider col, object target)
        {
            if (target is IFusible fusible)
            {
                CanInteract = false;
            }
        }

        private void OnAllFusiblesInserted()
        {
            // Handle quest completion logic
            Debug.Log("All fuses inserted!");
        }
    }
}