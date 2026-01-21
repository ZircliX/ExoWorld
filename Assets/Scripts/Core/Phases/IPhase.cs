using UnityEngine;

namespace OverBang.ExoWorld.Core
{
    public interface IPhase
    {
        Awaitable OnBegin();
        Awaitable Execute();
        Awaitable OnEnd();
    }
}