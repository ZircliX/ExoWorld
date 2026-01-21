namespace OverBang.ExoWorld.Core
{
    public abstract class PhaseListener<T> : IPhaseListener<T> where T : class, IPhase
    {
        protected T CurrentPhase { get; private set; }

        void IPhaseListener<T>.OnBegin(T phase)
        {
            if(CurrentPhase != null)
                return;
            
            CurrentPhase = phase;
            OnBegin(phase);
        }

        void IPhaseListener<T>.OnEnd(T phase)
        {
            if(phase != CurrentPhase)
                return;
            
            OnEnd(phase);
            CurrentPhase = null;
        }
        
        protected virtual void OnBegin(T phase) {}
        protected virtual void OnEnd(T phase) {}
    }
}