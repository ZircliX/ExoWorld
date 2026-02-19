using OverBang.ExoWorld.Core.Interactions;
using Unity.Netcode;
using UnityEngine;
using UnityUtils;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class Fusible : NetworkBehaviour, IInteractable, IFusible
    {
        [SerializeField] private QuestTwoData questTwoData;
        [SerializeField] private bool canBePickedUp = true;
        [SerializeField] private bool canBeDropped = true;
        
        private bool isPickedUp;
        private bool isUsable = true;
        private QuestTwoHandler questTwoHandler;

        public string InteractionText => "Ramasser le fusible";
        public int Priority { get; private set; } = (int)TargetPriority.Medium;
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

        private void Awake()
        {
            questTwoHandler ??= questTwoData.GetHandlerByData<QuestTwoHandler>();
            if (questTwoHandler == null)
            {
                gameObject.SetActive(false);
                canBePickedUp = false;
                canBeDropped = false;
                CanInteract = false;
            }
        }

        public void OnPickup(PlayerInteraction playerInteraction)
        {
            questTwoHandler ??= questTwoData.GetHandlerByData<QuestTwoHandler>();
            if (questTwoHandler is { StepIndex: < 1 })
            {
                UpdateHandlerStepRpc();
            }
            
            isPickedUp = true;
            CanInteract = false;
            SetPriority(-1);
            gameObject.SetActive(false); // Hide or trigger pickup animation
        }

        [Rpc(SendTo.Everyone)]
        private void UpdateHandlerStepRpc()
        {
            questTwoHandler.SetStepIndex(1);
        }

        public void OnDrop(PlayerInteraction playerInteraction)
        {
            if (!isUsable)
                return;
            
            isPickedUp = false;
            CanInteract = true;
            SetPriority((int)TargetPriority.Medium);
            transform.position = playerInteraction.CurrentInteractable.Instance.transform.position;
            gameObject.SetActive(true); // Show or trigger drop animation
        }

        public void SetPriority(int prio)
        {
            Priority = prio;
        }
        
        public void SetUsable(bool usable)
        {
            isUsable = usable;
        }
    }
}