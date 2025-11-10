using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
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