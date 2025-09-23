using OverBang.GameName.Hub;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Hub
{
    public class HubStartButton : HubListener
    {
        private bool canBeTriggered = false;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && canBeTriggered)
            {
                current.Complete(true);
            }
        }

        protected internal override void OnInit(HubPhase phase)
        {
            canBeTriggered = true;
        }

        protected internal override void OnRelease(HubPhase phase)
        {
            canBeTriggered = false;
        }
    }
}