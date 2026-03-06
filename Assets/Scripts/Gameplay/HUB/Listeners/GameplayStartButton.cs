using OverBang.ExoWorld.Core.Interactions;
using OverBang.ExoWorld.Core.Phases;
using OverBang.ExoWorld.Gameplay.Phase;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.HUB.Listeners
{
    public class GameplayStartButton : NetworkPhaseListener<HubPhase>, IInteractable
    {
        [SerializeField] private Transform target;
        
        public string InteractionText => "Démarrer le vaisseau";
        public int Priority => (int)TargetPriority.High;
        public bool CanInteract { get; private set; } = true;
        public InteractionType SupportedInteractions => InteractionType.Interact;
        Vector3 IInteractable.UIPosition => target.position;

        [Rpc(SendTo.Everyone)]
        private void ExitHubPhaseRpc()
        {
            CurrentPhase.Validate();
        }
        
        public void Interact(PlayerInteraction playerInteraction)
        {
            CanInteract = false;
            ExitHubPhaseRpc();
        }
    }
}