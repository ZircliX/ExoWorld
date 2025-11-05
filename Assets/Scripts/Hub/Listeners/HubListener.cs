using OverBang.GameName.Core.Phases;
using UnityEngine;

namespace OverBang.GameName.Hub
{
    public abstract class HubListener : MonoBehaviour, IPhaseListener<HubPhase>
    {
        protected HubPhase current;
        
        public void OnBegin(HubPhase phase)
        {
            if (current == null)
            {
                current = phase;
                Begin(phase);
            }
        }

        public void OnEnd(HubPhase phase, bool success)
        {
            if (current == phase)
            {
                current = null;
                End(phase, success);
            }
        }

        protected abstract void Begin(HubPhase phase);
        protected abstract void End(HubPhase phase, bool success);
    }
}