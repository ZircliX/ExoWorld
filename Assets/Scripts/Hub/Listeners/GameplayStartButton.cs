using OverBang.GameName.Core;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Hub
{
    public class GameplayStartButton : NetworkPhaseListener<HubPhase>
    {
        private bool canBeTriggered = false;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && canBeTriggered)
            {
                ExitHubPhaseRpc();
            }
        }

        [Rpc(SendTo.Everyone)]
        private void ExitHubPhaseRpc()
        {
            CurrentPhase.Validate();
        }

        protected override void OnBegin(HubPhase phase)
        {
            canBeTriggered = true;
        }

        protected override void OnEnd(HubPhase phase)
        {
            canBeTriggered = false;
        }
    }
}