using OverBang.ExoWorld.Core.Interactions;
using OverBang.ExoWorld.Gameplay.Player;
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
        private bool isConsumed;
        private readonly NetworkVariable<bool> isUsable = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        private QuestTwoHandler questTwoHandler;
        private PlayerEntity playerEntity;

        Vector3 IInteractable.UIPosition => transform.position.Add(y: 1f);
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
            if (!isUsable.Value)
                return;
            
            questTwoHandler ??= questTwoData.GetHandlerByData<QuestTwoHandler>();
            if (questTwoHandler is { StepIndex: < 1 })
            {
                UpdateHandlerStepRpc();
            }
            
            isPickedUp = true;
            CanInteract = false;
            
            playerEntity = playerInteraction.transform.parent.GetComponent<PlayerEntity>();
            playerEntity.ApplySpeed(-questTwoData.CarryingSlowForce, -1, nameof(Fusible));
            playerEntity.WeaponController.SetActiveState(false);
            
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
            if (!isUsable.Value) return;
            
            transform.position = playerEntity.transform.position;

            isPickedUp = false;
            playerEntity.WeaponController.SetActiveState(true);
            playerEntity.RemoveSpeed(nameof(Fusible));
            playerEntity = null;

            if (isConsumed) return; // don't reactivate if inserted into disjoncteur

            CanInteract = true;
            SetPriority((int)TargetPriority.Medium);
            gameObject.SetActive(true);
        }
        
        public void Consume()
        {
            isConsumed = true;
            isUsable.Value = false; // if owner
            gameObject.SetActive(false);
        }

        public void SetPriority(int prio)
        {
            Priority = prio;
        }
    }
}