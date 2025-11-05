using UnityEngine;

namespace OverBang.GameName.Core.Phases
{
    public interface IPhase
    {
        Awaitable OnBegin();
        Awaitable OnEnd(bool success);
    }
}