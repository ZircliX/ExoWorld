using UnityEngine;

namespace OverBang.GameName.Hub
{
    public abstract class HubListener : MonoBehaviour
    {
        protected internal HubPhase current;
        
        protected internal abstract void OnInit(HubPhase phase);
        protected internal abstract void OnRelease(HubPhase phase);
    }
}