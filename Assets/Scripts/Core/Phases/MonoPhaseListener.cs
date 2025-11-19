using UnityEngine;

namespace OverBang.GameName.Core
{
    public abstract class MonoPhaseListener<T> : MonoBehaviour, IPhaseListener<T> 
        where T : class, IPhase
    {
        protected T CurrentPhase { get; private set; }
        
        protected virtual void OnEnable()
        {
            this.Register();
        }

        protected virtual void OnDisable()
        {
            this.Unregister();
        }

        void IPhaseListener<T>.OnBegin(T phase)
        {
            if(CurrentPhase != null)
                return;
            
            CurrentPhase = phase;
            OnBegin(phase);
        }

        void IPhaseListener<T>.OnEnd(T phase)
        {
            if(CurrentPhase != phase)
                return;
            
            OnEnd(phase);
            CurrentPhase = null;
        }
        
        protected virtual void OnBegin(T phase) {}
        protected virtual void OnEnd(T phase) {}
    }
}