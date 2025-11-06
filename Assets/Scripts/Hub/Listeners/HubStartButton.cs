using OverBang.GameName.Core.Phases;
using OverBang.GameName.Hub;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Hub
{
    public class HubStartButton : MonoPhaseListener<HubPhase>
    {
        private bool canBeTriggered = false;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && canBeTriggered)
            {
                _ = currentPhase.End(true);
            }
        }

        protected override void Begin(HubPhase phase)
        {
            canBeTriggered = true;
        }

        protected override void End(HubPhase phase, bool success)
        {
            canBeTriggered = false;
        }
    }
}