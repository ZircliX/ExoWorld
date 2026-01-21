using OverBang.ExoWorld.Core;
using Unity.Netcode;
using UnityEngine;
using UnityUtils;

namespace OverBang.GameName.Hub
{
    public class GameplayStartButton : NetworkPhaseListener<HubPhase>, IInteractable
    {
        public string InteractionText => "Démarrer le vaisseau";
        public int Priority => (int)TargetPriority.High;
        public bool CanInteract { get; private set; } = true;
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