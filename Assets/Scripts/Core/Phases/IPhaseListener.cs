namespace OverBang.GameName.Core.Phases
{
    public interface IPhaseListener<in T> : IPhaseListener where T : IPhase
    {
        void OnBegin(T phase);
        void OnEnd(T phase, bool success);
    }

    /// <summary>
    /// DO NOT IMPLEMENT
    /// </summary>
    public interface IPhaseListener
    {
        
    }
}