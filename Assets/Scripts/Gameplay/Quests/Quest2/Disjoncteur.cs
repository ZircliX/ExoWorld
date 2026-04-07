using OverBang.ExoWorld.Core.Components;
using OverBang.ExoWorld.Core.Interactions;
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

        private int fusiblesInserted;
        private bool isHoldingFusible;
        private QuestTwoHandler questTwoHandler;
        private int currentPieces;
        
        public string InteractionText => CanInteract && isHoldingFusible ? questData.InteractionText : questData.InteractionTextEmpty;
        public int Priority => (int)TargetPriority.High;
        public bool CanInteract { get; private set; }
        public InteractionType SupportedInteractions { get; private set; } = InteractionType.Interact;

        Vector3 IInteractable.UIPosition => transform.position.Add(y: 1f);
        
        private void Awake()
        {
            questTwoHandler ??= questData.GetHandlerByData<QuestTwoHandler>();

            if (questTwoHandler == null)
            {
                gameObject.SetActive(false);
                CanInteract = false;
                SupportedInteractions = InteractionType.None;
                return;
            }
            
            detectionArea.GetCollider<SphereCollider>().radius = questData.InteractionRange;
            detectionArea.SetRequireInterface<IFusible>();
        }
        
        public void Interact(PlayerInteraction playerInteraction)
        {
            questTwoHandler ??= questData.GetHandlerByData<QuestTwoHandler>();
            if (questTwoHandler is { StepIndex: < 2 })
                UpdateHandlerStepRpc();
            
            AddFusible(playerInteraction);
        }

        [Rpc(SendTo.Everyone)]
        private void UpdateHandlerStepRpc()
        {
            questTwoHandler.SetStepIndex(2);
        }

        public void OnPlayerEnter(PlayerInteraction playerInteraction)
        {
            isHoldingFusible = playerInteraction.GetHoldingItemType<IFusible>();
            CanInteract = isHoldingFusible;
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

        private void AddFusible(PlayerInteraction playerInteraction)
        {
            if (!playerInteraction.IsHoldingItem() || !IsOwner)
                return;

            if (isHoldingFusible)
            {
                isHoldingFusible = false;
                CanInteract = false;
                fusiblesInserted++;

                if (playerInteraction.HeldItem.Instance is Fusible fusible)
                {
                    fusible.ConsumeRpc();
                    playerInteraction.DropItem();
                }
                
                playerInteraction.RequestUpdateUI(this);

                DispatchAddedFusibleRpc();

                if (fusiblesInserted >= questData.TotalPieces)
                {
                    OnAllFusiblesInserted();
                }
            }
        }

        [Rpc(SendTo.Everyone)]
        private void DispatchAddedFusibleRpc()
        {
            currentPieces++;
            QuestTwoEvent evt = new QuestTwoEvent(currentPieces, questData.TotalPieces);
            ObjectivesManager.DispatchGameEvent(evt);
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