using OverBang.ExoWorld.Core.Interactions;
using OverBang.ExoWorld.Core.Phases;
using OverBang.ExoWorld.Core.Utils;
using OverBang.ExoWorld.Gameplay.Phase;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.HUB
{
    public class Ship : NetworkPhaseListener<GameplayPhase>, IInteractable
    {
        [SerializeField] private Transform uiPosition;
        
        public string InteractionText => "Démarrer la capsule";
        public int Priority => (int)TargetPriority.Medium;
        public bool CanInteract { get; private set; } = true;
        public InteractionType SupportedInteractions => InteractionType.Interact;
        Vector3 IInteractable.UIPosition => uiPosition.position;

        public void Interact(PlayerInteraction playerInteraction)
        {
            if (!CanInteract) 
                return;
            
            ExitHubPhaseRpc();
        }
        
        [Rpc(SendTo.Everyone)]
        private void ExitHubPhaseRpc()
        {
            if (CurrentPhase == null)
                return;
            
            Debug.Log("Exiting hub phase on client : " + SessionManager.Global.CurrentPlayer.Id);
            CanInteract = false;
            CurrentPhase.SetIsDone();
        }
    }
}