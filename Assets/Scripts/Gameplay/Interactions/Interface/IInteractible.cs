using UnityEngine;

namespace OverBang.GameName.Gameplay.Interface
{
    public interface IInteractable
    {
        string InteractionText { get; }
        int Priority { get; }
        bool CanInteract { get; }

        Vector3 UIPosition => transform.position;
        // ReSharper disable once InconsistentNaming
        Transform transform { get; }

        void OnPlayerEnter(PlayerInteraction playerInteraction)
        {
            
        }
        void OnPlayerExit(PlayerInteraction playerInteraction)
        {
            
        }
        void Interact(PlayerInteraction playerInteraction);
    }
}