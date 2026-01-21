namespace OverBang.ExoWorld.Core
{
    public interface IPhaseListener<in T> : IPhaseListener where T : IPhase
    {
        void OnBegin(T phase);
        void OnEnd(T phase);
    }

    /// <summary>
    /// DO NOT IMPLEMENT
    /// </summary>
    public interface IPhaseListener
    {
        
    }
}