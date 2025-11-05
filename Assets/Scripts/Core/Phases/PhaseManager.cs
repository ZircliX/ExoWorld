using System.Collections.Generic;

namespace OverBang.GameName.Core.Phases
{
    public class PhaseManager
    {
        public static PhaseManager Global => GameController.PhaseManager;

        private readonly HashSet<IPhaseListener> listeners = new HashSet<IPhaseListener>();

        public bool RegisterListener<T>(IPhaseListener<T> listener) where T : IPhase
        {
            return listeners.Add(listener);
        }

        public bool UnregisterListener<T>(IPhaseListener<T> listener) where T : IPhase
        {
            return listeners.Remove(listener);
        }

        internal void OnBeginPhase<T>(T phase) where T : IPhase
        {
            foreach (IPhaseListener phaseListener in listeners)
            {
                if (phaseListener is IPhaseListener<T> phaseListenerT)
                {
                    phaseListenerT.OnBegin(phase);
                }
            }
        }

        internal void OnEndPhase<T>(T phase, bool success) where T : IPhase
        {
            foreach (IPhaseListener phaseListener in listeners)
            {
                if (phaseListener is IPhaseListener<T> phaseListenerT)
                {
                    phaseListenerT.OnEnd(phase, success);
                }
            }
        }
    }
}