using OverBang.GameName.Managers;
using OverBang.GameName.Offline;
using UnityEngine;

namespace OverBang.GameName.Debug
{
    public class DebugHubScene : MonoBehaviour
    {
        [SerializeField] private int difficulty = 0;
        
        private void Start()
        {
            if (GameController.CurrentGameMode == null)
            {
                OfflineGameMode offlineGameMode = OfflineGameMode.Create(0, difficulty);
                offlineGameMode.SetGameMode();
            }
            else
            {
                Destroy(this);
            }
        }
    }
}