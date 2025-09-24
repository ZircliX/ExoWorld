using UnityEngine;

namespace OverBang.GameName.Core.GameMode
{
    public interface IGameMode
    {
        Awaitable Run();
    }
}