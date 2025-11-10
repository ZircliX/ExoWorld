using OverBang.GameName.Core;
using OverBang.GameName.Gameplay;
using UnityEngine;

namespace OverBang.GameName.Debug
{
    public class DebugMapScene : MonoBehaviour
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
