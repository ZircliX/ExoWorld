using OverBang.ExoWorld.Core.Phases;
using OverBang.ExoWorld.Core.Utils;
using OverBang.ExoWorld.Gameplay.Phase;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.HUB
{
    public class Ship : NetworkPhaseListener<GameplayPhase>
    {
        private bool canBeTriggered;
        
        private void OnTriggerEnter(Collider other)
        {
            if ((other.CompareTag("Player") || other.CompareTag("LocalPlayer")) && canBeTriggered)
            {
                ExitHubPhaseRpc();
            }
        }
        
        [Rpc(SendTo.Everyone)]
        private void ExitHubPhaseRpc()
        {
            if (CurrentPhase == null)
                return;
            
            Debug.Log("Exiting hub phase on client : " + SessionManager.Global.CurrentPlayer.Id);
            canBeTriggered = false;
            CurrentPhase.SetIsDone();
        }

        public void SetCanBeTriggered(bool value)
        {
            canBeTriggered = value;
        }
    }
}