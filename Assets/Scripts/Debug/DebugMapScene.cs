using OverBang.GameName.Offline;
using UnityEngine;

namespace OverBang.GameName.Debug
{
    public class DebugMapScene : MonoBehaviour
    {
        private void Awake()
        {
            if (GameController.CurrentGameMode == null)
            {
                OfflineGameMode offlineGameMode = OfflineGameMode.Create();
                offlineGameMode.SetGameMode();
            }
            else
            {
                Destroy(this);
            }
        }
    }
}
