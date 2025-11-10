using OverBang.GameName.Offline;
using UnityEngine;

namespace OverBang.GameName.Debug
{
    public class DebugHubScene : MonoBehaviour
    {
        private void Awake()
        {
            if (GameController.CurrentGameMode == null)
            {
                SurvivalGameMode survivalGameMode = SurvivalGameMode.Create();
                survivalGameMode.SetGameMode();
            }
            else
            {
                Destroy(this);
            }
        }
    }
}