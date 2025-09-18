using OverBang.GameName.Core.GameMode;
using OverBang.GameName.Managers;
using UnityEngine;

namespace OverBang.GameName.Online
{
    public class StartOnlineMode : MonoBehaviour
    {
        public void StartMode()
        {
            IGameMode onlineGameMode = new OnlineGameMode();
            onlineGameMode.SetGameMode();
        }
    }
}
