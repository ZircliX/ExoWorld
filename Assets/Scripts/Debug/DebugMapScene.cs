using OverBang.GameName.Offline;
using UnityEngine;

namespace OverBang.GameName.Debug
{
    public class DebugMapScene : MonoBehaviour
    {
        [SerializeField] private int difficulty = 0;
        
        private void Awake()
        {
            if (GameController.CurrentGameMode == null)
            {
                OfflineGameMode offlineGameMode = OfflineGameMode.Create(0, difficulty);
                offlineGameMode.SetGameMode();
                //offlineGameMode.StateMachine.ChangeState(new GameplayState(offlineGameMode.StateMachine, offlineGameMode, offlineGameMode));
            }
            else
            {
                Destroy(this);
            }
        }
    }
}
