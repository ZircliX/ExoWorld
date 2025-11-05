using OverBang.GameName.Core.Phases;

namespace OverBang.GameName.Gameplay.Gameplay.Listeners
{
    public abstract class PhaseListener<T> : IPhaseListener<T> where T : IPhase
    {
        protected T currentPhase;

        public void OnBegin(T phase)
        {
            currentPhase = phase;
            Begin(phase);
        }

        public void OnEnd(T phase, bool success)
        {
            currentPhase = default;
            End(phase, success);
        }

        protected virtual void Begin(T phase) {}
        protected virtual void End(T phase, bool success) {}
    }
}