using UnityEngine;

namespace OverBang.ExoWorld.Core.Interactions
{
    public class InteractableData
    {
        public IInteractable Instance { get; private set; }
        public string InteractionText { get; private set; }
        public int Priority { get; private set; }
        public bool CanInteract { get; private set; }
        public InteractionType SupportedInteractions { get; private set; }
        public Vector3 LastKnownPosition { get; private set; }
        public bool IsHeld { get; set; }
        
        public InteractableData(IInteractable interactable)
        {
            Instance = interactable;
            UpdateData();
        }

        public void UpdateData()
        {
            if (Instance == null) return;

            InteractionText = Instance.InteractionText;
            Priority = Instance.Priority;
            CanInteract = Instance.CanInteract;
            SupportedInteractions = Instance.SupportedInteractions;
            LastKnownPosition = Instance.UIPosition;
        }

        public bool SupportsInteractionType(InteractionType interactionType)
        {
            return (SupportedInteractions & interactionType) != 0;
        }

        public bool IsValid()
        {
            return Instance != null;
        }
    }
}