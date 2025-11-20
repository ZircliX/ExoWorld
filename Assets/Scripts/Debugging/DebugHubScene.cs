using OverBang.GameName.Core;
using OverBang.GameName.Gameplay;
using UnityEngine;

namespace OverBang.GameName.Debugging
{
    [DefaultExecutionOrder(-10)]

public class DebugHubScene : MonoBehaviour
    {
        private void Awake()
        {
            if (GameController.CurrentGameMode == null)
            {
                Debug.LogWarning($"No gamemode selected, Starting SurvivalGameMode from {nameof(DebugHubScene)}");
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