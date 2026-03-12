using OverBang.ExoWorld.Core.Interactions;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class PumpStart : MonoBehaviour, IInteractable
    {
        [SerializeField] private Pump pump;
        [SerializeField] private Transform target;

        public string InteractionText => string.Empty;
        public int Priority => (int)TargetPriority.High;
        public bool CanInteract => !pump.IsStarted && !pump.IsCompleted;
        public InteractionType SupportedInteractions => InteractionType.Interact;

        Vector3 IInteractable.UIPosition => target.position;

        public void Interact(PlayerInteraction playerInteraction)
        {
            pump.CallStartRepair();
        }
    }
}