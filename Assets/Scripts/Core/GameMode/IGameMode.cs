using UnityEngine;

namespace OverBang.GameName.Core
{
    public interface IGameMode
    {
        Awaitable Run();
    }
}