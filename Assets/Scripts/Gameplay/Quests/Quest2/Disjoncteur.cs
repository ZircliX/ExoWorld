using System.Collections.Generic;
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
        [SerializeField] private List<Fusible> fusibles;

        private int fusiblesInserted = 0;

        public string InteractionText => CanInteract ? questData.InteractionText : questData.InteractionTextEmpty;
        public int Priority => (int)TargetPriority.High;
        public bool CanInteract { get; private set; } = false;
        public InteractionType SupportedInteractions => InteractionType.Interact;

        Vector3 IInteractable.UIPosition => transform.position.Add(y: 1f, x: -1f);

        public void Interact(PlayerInteraction playerInteraction)
        {
            AddFusible(playerInteraction);
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

            InteractableData heldData = playerInteraction.HeldItem;
            if (heldData.Instance is IFusible fusible)
            {
                fusiblesInserted++;
                playerInteraction.DropItem();

                QuestTwoEvent evt = new QuestTwoEvent(1);
                ObjectivesManager.DispatchGameEvent(evt);

                if (fusiblesInserted >= fusibles.Count)
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