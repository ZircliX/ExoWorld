using OverBang.ExoWorld.Core.Interactions;
using OverBang.ExoWorld.Core.Phases;
using OverBang.ExoWorld.Gameplay.Phase;
using Unity.Netcode;
using UnityEngine;
using UnityUtils;

namespace OverBang.ExoWorld.Gameplay.HUB.Listeners
{
    public class GameplayStartButton : NetworkPhaseListener<HubPhase>, IInteractable
    {
        public string InteractionText => "Démarrer le vaisseau";
        public int Priority => (int)TargetPriority.High;
        public bool CanInteract { get; private set; } = true;
        public InteractionType SupportedInteractions => InteractionType.Interact;
        Vector3 IInteractable.UIPosition => transform.position.Add(y: 0.6f);

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