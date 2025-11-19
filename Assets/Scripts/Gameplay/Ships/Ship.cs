using OverBang.GameName.Core;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class Ship : NetworkPhaseListener<GameplayPhase>
    {
        [SerializeField] private GameObject go;
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
            if (CurrentPhase == null)
                return;
            
            Debug.Log("Exiting hub phase on client : " + SessionManager.Global.CurrentPlayer.Id);
            CurrentPhase.SetIsDone();
        }
        
        protected override void OnBegin(GameplayPhase phase)
        {
            canBeTriggered = true;
        }

        protected override void OnEnd(GameplayPhase phase)
        {
            canBeTriggered = false;
        }
    }
}