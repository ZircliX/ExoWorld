using OverBang.GameName.Core;
using OverBang.GameName.Gameplay;
using UnityEngine;

namespace OverBang.GameName.Debugging
{
    [DefaultExecutionOrder(-999)]
    public class DebugMapScene : MonoBehaviour
    {
        private void Awake()
        {
            if (GameController.CurrentGameMode == null)
            {
                Debug.LogError($"No gamemode selected, Starting SurvivalGameMode from {nameof(DebugMapScene)}");
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
