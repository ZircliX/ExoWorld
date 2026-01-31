using UnityEngine;

namespace OverBang.ExoWorld.Core.Interactions
{
    public interface IInteractable
    {
        string InteractionText { get; }
        int Priority { get; }
        bool CanInteract { get; }
        InteractionType SupportedInteractions { get; }

        Vector3 UIPosition => transform.position;
        // ReSharper disable once InconsistentNaming
        Transform transform { get; }

        void OnPlayerEnter(PlayerInteraction playerInteraction) { }
        void OnPlayerExit(PlayerInteraction playerInteraction) { }
        void Interact(PlayerInteraction playerInteraction) { }
        void OnPickup(PlayerInteraction playerInteraction) { }
        void OnDrop(PlayerInteraction playerInteraction) { }
    }
}