using UnityEngine;

namespace OverBang.GameName.Core.Phases
{
    public abstract class MonoPhaseListener<T> : MonoBehaviour, IPhaseListener<T> where T : IPhase
    {
        protected T phase;
        
        protected virtual void OnEnable() => this.Register();
        protected virtual void OnDisable() => this.Unregister();

        public void OnBegin(T phase)
        {
            this.phase = phase;
            Begin();
        }

        public void OnEnd(T phase, bool success)
        {
            End(success);
        }
        
        protected virtual void Begin() {}
        protected virtual void End(bool success) {}
    }
}