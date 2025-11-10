using UnityEngine;

namespace OverBang.GameName.Core
{
    public abstract class MonoPhaseListener<T> : MonoBehaviour, IPhaseListener<T> where T : IPhase
    {
        protected T currentPhase;
        
        protected virtual void OnEnable() => this.Register();
        protected virtual void OnDisable() => this.Unregister();

        public void OnBegin(T phase)
        {
            currentPhase = phase;
            Begin(phase);
        }

        public void OnEnd(T phase, bool success)
        {
            End(phase, success);
        }
        
        protected virtual void Begin(T phase) {}
        protected virtual void End(T phase, bool success) {}
    }
}