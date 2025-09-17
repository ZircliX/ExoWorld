using OverBang.GameName.Core.Core.GameMode;
using UnityEngine;

namespace OverBang.GameName.Online
{
    public class OnlineGameMode : IGameMode
    {
        public void Activate()
        {
            Debug.Log("Starting ONLINE game mode");
        }

        public void Deactivate()
        {
            Debug.Log("Deactivating ONLINE game mode");
        }
    }
}