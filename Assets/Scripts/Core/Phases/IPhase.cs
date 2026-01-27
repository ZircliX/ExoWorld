using UnityEngine;

namespace OverBang.ExoWorld.Core.Phases
{
    public interface IPhase
    {
        Awaitable OnBegin();
        Awaitable Execute();
        Awaitable OnEnd();
    }
}