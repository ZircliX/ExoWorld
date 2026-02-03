using UnityEngine;

namespace OverBang.ExoWorld.Core.Interactions
{
    public class InteractableData
    {
        public IInteractable Instance { get; private set; }
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

            LastKnownPosition = Instance.UIPosition;
        }

        public bool SupportsInteractionType(InteractionType interactionType)
        {
            return (Instance.SupportedInteractions & interactionType) != 0;
        }

        public bool IsValid()
        {
            return Instance != null;
        }
    }
}