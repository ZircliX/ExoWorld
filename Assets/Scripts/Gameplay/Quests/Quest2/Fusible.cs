using OverBang.ExoWorld.Core.Interactions;
using OverBang.ExoWorld.Gameplay.Abilities;
using Unity.Netcode;
using UnityEngine;
using UnityUtils;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class Fusible : NetworkBehaviour, IInteractable, IFusible
    {
        [SerializeField] private QuestTwoData questTwoData;
        [SerializeField] private DetectionArea detectionArea;
        [SerializeField] private bool canBePickedUp = true;
        [SerializeField] private bool canBeDropped = true;

        private Vector3 originalPosition;
        private bool isPickedUp = false;

        private QuestTwoHandler questTwoHandler;

        public string InteractionText => isPickedUp ? "Déposer le fusible" : "Ramasser le fusible";
        public int Priority => (int)TargetPriority.Medium;
        public bool CanInteract { get; private set; } = true;
        public InteractionType SupportedInteractions
        {
            get
            {
                InteractionType interactions = InteractionType.None;
                if (canBePickedUp) interactions |= InteractionType.Pickup;
                if (canBeDropped && isPickedUp) interactions |= InteractionType.Drop;
                return interactions;
            }
        }

        Vector3 IInteractable.UIPosition => transform.position.Add(y: 1f);
        
        public void OnPickup(PlayerInteraction playerInteraction)
        {
            questTwoHandler ??= questTwoData.GetHandlerByData<QuestTwoHandler>();
            if (questTwoHandler is { StepIndex: < 1 })
            {
                questTwoHandler.SetStepIndex(1);
            }
            
            isPickedUp = true;
            CanInteract = false;
            gameObject.SetActive(false); // Hide or trigger pickup animation
        }

        public void OnDrop(PlayerInteraction playerInteraction)
        {
            isPickedUp = false;
            CanInteract = true;
            transform.position = playerInteraction.CurrentInteractable.Instance.transform.position;
            gameObject.SetActive(true); // Show or trigger drop animation
        }

        private void Awake()
        {
            originalPosition = transform.position;
            detectionArea.SetAllowedTags("Player", "LocalPlayer");
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

        private void OnEnter(Collider col, object target)
        {
            CanInteract = true;
        }

        private void OnExit(Collider col, object target)
        {
            CanInteract = false;
        }

        public void ResetPosition()
        {
            isPickedUp = false;
            transform.position = originalPosition;
            gameObject.SetActive(true);
        }
    }
}