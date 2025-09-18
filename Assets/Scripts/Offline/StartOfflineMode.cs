using OverBang.GameName.Core.GameMode;
using OverBang.GameName.Managers;
using UnityEngine;

namespace OverBang.GameName.Offline
{
    public class StartOfflineMode : MonoBehaviour
    {
        public void StartMode()
        {
            IGameMode offlineGameMode = OfflineGameMode.Create(0, 0);
            offlineGameMode.SetGameMode();
        }
    }
}