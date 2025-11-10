using OverBang.GameName.Core.GameMode;
using UnityEngine;

namespace OverBang.GameName.Offline
{
    public class StartSurvivalGameMode : MonoBehaviour
    {
        public void StartMode()
        {
            IGameMode offlineGameMode = SurvivalGameMode.Create();
            offlineGameMode.SetGameMode();
        }
    }
}