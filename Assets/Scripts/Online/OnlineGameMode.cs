using OverBang.GameName.Core.GameMode;
using UnityEngine;

namespace OverBang.GameName.Online
{
    public class OnlineGameMode : IGameMode
    {
        public Awaitable Run()
        {
            Debug.Log("Running Online Game Mode...");
            // Implement online game mode logic here
            return Awaitable.EndOfFrameAsync();
        }
    }
}