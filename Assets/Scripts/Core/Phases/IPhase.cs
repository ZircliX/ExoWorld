using UnityEngine;

namespace OverBang.GameName.Core
{
    public interface IPhase
    {
        Awaitable OnBegin();
        Awaitable Execute();
        Awaitable OnEnd();
    }
}